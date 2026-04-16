using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using E_comma.Models;

namespace E_comma.Handlers
{
    public class Chatbot : IHttpHandler
    {
        private const int MaxMessageLength = 800;
        private const int MaxMessages = 10;
        private const int CatalogProductLimit = 30;
        private const int DefaultMaxProductResults = 6;
        private const int WebSearchSnippetLimit = 2;
        private const int MaxDescriptionLength = 280;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetNoStore();

            if (!string.Equals(context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 405;
                context.Response.Write("{\"error\":\"Method not allowed\"}");
                return;
            }

            string body;
            using (var reader = new StreamReader(context.Request.InputStream))
            {
                body = reader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                WriteError(context, "Empty request body");
                return;
            }

            var serializer = new JavaScriptSerializer();
            ChatRequest request;

            try
            {
                request = serializer.Deserialize<ChatRequest>(body);
            }
            catch
            {
                WriteError(context, "Invalid JSON");
                return;
            }

            if (request == null || request.Messages == null || request.Messages.Count == 0)
            {
                WriteError(context, "No messages provided");
                return;
            }

            var sanitizedMessages = SanitizeMessages(request.Messages);
            if (sanitizedMessages.Count == 0)
            {
                WriteError(context, "No valid messages");
                return;
            }

            string lastUserMessage = sanitizedMessages.LastOrDefault(m => m.Role == "user")?.Content ?? string.Empty;
            string actionReply;
            if (TryHandleActionRequest(lastUserMessage, out actionReply))
            {
                var actionResponse = new { reply = actionReply };
                context.Response.Write(serializer.Serialize(actionResponse));
                return;
            }
            string catalogReply;
            if (TryHandleProductQuery(lastUserMessage, out catalogReply))
            {
                var catalogResponse = new { reply = catalogReply };
                context.Response.Write(serializer.Serialize(catalogResponse));
                return;
            }

            string reply;
            try
            {
                string apiKey = ConfigurationManager.AppSettings["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
                }

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    WriteError(context, "API key not configured");
                    return;
                }

                string apiUrl = ConfigurationManager.AppSettings["Gemini:ApiUrl"] ??
                                "https://generativelanguage.googleapis.com/v1beta/models";
                string modelSetting = ConfigurationManager.AppSettings["Gemini:Model"] ?? "gemini-flash-latest";
                string systemPrompt = ConfigurationManager.AppSettings["Gemini:SystemPrompt"] ??
                                      "Tu es l'assistant de la boutique E-comma. La boutique vend des produits cosmetiques et de bien-etre. Reponds en francais, court et utile. Si un produit n'est pas dans le catalogue, dis que tu n'as pas l'information et propose de consulter la boutique.";
                string catalogContext = BuildCatalogContext();
                string fullPrompt = systemPrompt;
                if (!string.IsNullOrWhiteSpace(catalogContext))
                {
                    fullPrompt += "\n\nCatalogue:\n" + catalogContext;
                }

                List<string> modelCandidates = ParseModelCandidates(modelSetting);
                bool useSystemInstruction = !string.IsNullOrWhiteSpace(fullPrompt) &&
                                            apiUrl.IndexOf("v1beta", StringComparison.OrdinalIgnoreCase) >= 0;

                IEnumerable<ChatMessage> messagesForGemini = sanitizedMessages;
                if (!useSystemInstruction && !string.IsNullOrWhiteSpace(fullPrompt))
                {
                    messagesForGemini = new[] { new ChatMessage { Role = "user", Content = fullPrompt } }
                        .Concat(sanitizedMessages);
                }

                var geminiContents = messagesForGemini.Select(m => new
                {
                    role = m.Role == "assistant" ? "model" : "user",
                    parts = new[] { new { text = m.Content } }
                }).ToList();

                var geminiPayload = new Dictionary<string, object>
                {
                    { "contents", geminiContents },
                    { "generationConfig", new { temperature = 0.4, maxOutputTokens = 300 } }
                };

                if (useSystemInstruction)
                {
                    geminiPayload["system_instruction"] = new { parts = new[] { new { text = fullPrompt } } };
                }

                string serializedPayload = serializer.Serialize(geminiPayload);
                string lastModelError = null;
                reply = null;

                foreach (string modelName in modelCandidates)
                {
                    string endpoint = BuildGeminiEndpoint(apiUrl, modelName, apiKey);
                    try
                    {
                        reply = SendGeminiRequest(endpoint, serializedPayload);
                        lastModelError = null;
                        break;
                    }
                    catch (InvalidOperationException ex)
                    {
                        lastModelError = ex.Message;
                        if (IsModelUnavailableError(ex.Message))
                        {
                            continue;
                        }
                        throw;
                    }
                }

                if (string.IsNullOrWhiteSpace(reply))
                {
                    string message = "No available Gemini models for generateContent.";
                    if (!string.IsNullOrWhiteSpace(lastModelError))
                    {
                        message += " Last error: " + lastModelError;
                    }
                    message += " Tried: " + string.Join(", ", modelCandidates);
                    WriteError(context, message);
                    return;
                }
            }
            catch (Exception ex)
            {
                WriteError(context, "Chatbot error: " + ex.Message);
                return;
            }

            var responsePayload = new { reply = reply };
            context.Response.Write(serializer.Serialize(responsePayload));
        }

        private static List<ChatMessage> SanitizeMessages(IEnumerable<ChatMessage> messages)
        {
            var output = new List<ChatMessage>();

            foreach (var message in messages)
            {
                if (message == null || string.IsNullOrWhiteSpace(message.Content))
                    continue;

                string role = (message.Role ?? string.Empty).Trim().ToLowerInvariant();
                if (role != "user" && role != "assistant")
                    continue;

                string content = message.Content.Trim();
                if (content.Length > MaxMessageLength)
                {
                    content = content.Substring(0, MaxMessageLength);
                }

                output.Add(new ChatMessage { Role = role, Content = content });
            }

            if (output.Count > MaxMessages)
            {
                output = output.Skip(output.Count - MaxMessages).ToList();
            }

            return output;
        }

        private static string SendGeminiRequest(string apiUrl, string json)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", "E-comma");

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response = http.PostAsync(apiUrl, content).Result;
                    var responseBody = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorDetails = ExtractErrorMessage(responseBody);
                        string message = "API request failed (" + (int)response.StatusCode + ")";
                        if (!string.IsNullOrWhiteSpace(errorDetails))
                        {
                            message += ": " + errorDetails;
                        }
                        throw new InvalidOperationException(message);
                    }

                    var serializer = new JavaScriptSerializer();
                    var parsed = serializer.Deserialize<GeminiResponse>(responseBody);
                    var reply = parsed?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

                    if (string.IsNullOrWhiteSpace(reply))
                    {
                        return "Desole, je n'ai pas compris. Pouvez-vous reformuler ?";
                    }

                    return reply.Trim();
                }
            }
        }

        private static string BuildGeminiEndpoint(string apiUrl, string model, string apiKey)
        {
            string endpoint = string.IsNullOrWhiteSpace(apiUrl)
                ? "https://generativelanguage.googleapis.com/v1beta/models"
                : apiUrl.Trim();

            if (endpoint.IndexOf(":generateContent", StringComparison.OrdinalIgnoreCase) < 0)
            {
                string normalizedModel = (model ?? string.Empty).Trim();
                if (normalizedModel.StartsWith("models/", StringComparison.OrdinalIgnoreCase))
                {
                    normalizedModel = normalizedModel.Substring("models/".Length);
                }
                if (string.IsNullOrWhiteSpace(normalizedModel))
                {
                    normalizedModel = "gemini-1.5-flash";
                }

                endpoint = endpoint.TrimEnd('/') + "/" + normalizedModel + ":generateContent";
            }

            string separator = endpoint.Contains("?") ? "&" : "?";
            endpoint += separator + "key=" + HttpUtility.UrlEncode(apiKey);
            return endpoint;
        }

        private static string BuildCatalogContext()
        {
            try
            {
                var categories = Category.GetAll();
                var products = ProductExtended.GetCatalogSnapshot(CatalogProductLimit);
                var builder = new StringBuilder();

                if (categories != null && categories.Count > 0)
                {
                    builder.AppendLine("Categories: " + string.Join(", ", categories.Select(c => c.Name)));
                }

                if (products != null && products.Count > 0)
                {
                    builder.AppendLine("Produits:");
                    foreach (var product in products)
                    {
                        string brand = string.IsNullOrWhiteSpace(product.Brand) ? "" : product.Brand;
                        string category = string.IsNullOrWhiteSpace(product.CategoryName) ? "" : product.CategoryName;
                        string price = product.BasePrice > 0
                            ? product.BasePrice.ToString("0.##", CultureInfo.InvariantCulture) + " DH"
                            : "";

                        var parts = new List<string>();
                        if (!string.IsNullOrWhiteSpace(product.Name))
                            parts.Add(product.Name);
                        if (!string.IsNullOrWhiteSpace(brand))
                            parts.Add(brand);
                        if (!string.IsNullOrWhiteSpace(category))
                            parts.Add(category);
                        if (!string.IsNullOrWhiteSpace(price))
                            parts.Add(price);

                        if (parts.Count > 0)
                        {
                            builder.AppendLine("- " + string.Join(" | ", parts));
                        }
                    }
                }

                return builder.ToString().Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private static bool TryHandleProductQuery(string message, out string reply)
        {
            reply = null;

            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            try
            {
                if (!LooksLikeProductQuery(message))
                {
                    return false;
                }

                int maxResults = GetIntSetting("Chatbot:MaxProductResults", DefaultMaxProductResults);
                var keywords = ExtractKeywords(message);
                List<ProductExtended> products = keywords.Count == 0
                    ? ProductExtended.GetCatalogSnapshot(maxResults)
                    : ProductExtended.SearchForChatbot(keywords, maxResults);

                if (products.Count == 0)
                {
                    reply = "Desole, aucun produit correspondant dans le catalogue.";
                    return true;
                }

                int bestScore;
                ProductExtended bestMatch = SelectBestMatch(products, keywords, out bestScore);
                bool isInfoRequest = LooksLikeInfoRequest(message);
                bool strongMatch = IsStrongMatch(bestMatch, message, keywords, bestScore);

                if (bestMatch != null && (isInfoRequest || strongMatch))
                {
                    var detailed = ProductExtended.GetDetailedById(bestMatch.Id) ?? bestMatch;
                    string webInfo = GetWebInfoForProduct(detailed);
                    reply = FormatSingleProductResponse(detailed, webInfo);
                    return true;
                }

                reply = FormatProductListResponse(products);
                return true;
            }
            catch
            {
                reply = null;
                return false;
            }
        }

        private static bool TryHandleActionRequest(string message, out string reply)
        {
            reply = null;

            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            string normalized = NormalizeText(message);
            string[] triggers =
            {
                "ajouter au panier", "ajoute au panier", "panier", "commander", "commande", "acheter",
                "payer", "paiement", "annuler", "modifier", "supprimer", "appliquer code", "code promo",
                "livraison", "adresse", "confirmer"
            };

            foreach (string trigger in triggers)
            {
                if (normalized.Contains(trigger))
                {
                    reply = "Je ne peux pas effectuer d'actions (ajouter au panier, commander ou modifier). Utilisez les boutons du site pour ces actions.";
                    return true;
                }
            }

            return false;
        }

        private static bool LooksLikeProductQuery(string message)
        {
            string normalized = NormalizeText(message);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            string[] triggers =
            {
                "produit", "produits", "catalogue", "boutique", "prix", "marque", "disponible", "stock",
                "recommande", "recommander", "suggestion", "suggere", "montre", "montrez", "liste",
                "creme", "serum", "parfum", "soin", "shampoing", "maquillage", "anti age", "anti-age"
            };

            foreach (string trigger in triggers)
            {
                if (normalized.Contains(trigger))
                {
                    return true;
                }
            }

            var categories = Category.GetAll();
            foreach (var category in categories)
            {
                string categoryName = NormalizeText(category.Name);
                if (!string.IsNullOrWhiteSpace(categoryName) && normalized.Contains(categoryName))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool LooksLikeInfoRequest(string message)
        {
            string normalized = NormalizeText(message);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            string[] triggers =
            {
                "info", "infos", "information", "details", "detail", "composition", "ingredients",
                "utilisation", "usage", "comment", "prix", "a quoi sert", "effet"
            };

            foreach (string trigger in triggers)
            {
                if (normalized.Contains(trigger))
                {
                    return true;
                }
            }

            return false;
        }

        private static List<string> ExtractKeywords(string message)
        {
            string normalized = NormalizeText(message);
            char[] separators =
            {
                ' ', '\t', '\r', '\n', '.', ',', ';', ':', '!', '?', '/', '\\', '-', '_',
                '(', ')', '[', ']', '{', '}', '\"', '\''
            };

            var stopwords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "le", "la", "les", "un", "une", "des", "de", "du", "et", "ou", "pour", "avec", "sur",
                "dans", "je", "tu", "il", "elle", "nous", "vous", "ils", "elles", "mon", "ma", "mes",
                "ton", "ta", "tes", "son", "sa", "ses", "ce", "cet", "cette", "ces", "plus", "moins",
                "prix", "marque", "disponible", "stock", "produit", "produits", "montre", "montrez",
                "recommande", "recommander", "suggestion", "suggere", "chercher", "cherche", "veux",
                "voudrais", "peux", "peut", "svp", "stp"
            };

            var keywords = new List<string>();
            foreach (string part in normalized.Split(separators, StringSplitOptions.RemoveEmptyEntries))
            {
                if (part.Length < 3)
                {
                    continue;
                }

                if (stopwords.Contains(part))
                {
                    continue;
                }

                keywords.Add(part);
            }

            return keywords;
        }

        private static ProductExtended SelectBestMatch(List<ProductExtended> products, List<string> keywords, out int bestScore)
        {
            bestScore = 0;
            if (products == null || products.Count == 0)
            {
                return null;
            }

            ProductExtended best = null;
            foreach (var product in products)
            {
                string haystack = NormalizeText((product.Name ?? "") + " " + (product.Brand ?? "") + " " + (product.CategoryName ?? ""));
                int score = 0;
                foreach (string keyword in keywords)
                {
                    if (haystack.Contains(keyword))
                    {
                        score++;
                    }
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    best = product;
                }
            }

            return best;
        }

        private static bool IsStrongMatch(ProductExtended product, string message, List<string> keywords, int bestScore)
        {
            if (product == null)
            {
                return false;
            }

            string normalizedMessage = NormalizeText(message);
            string normalizedName = NormalizeText(product.Name ?? string.Empty);

            if (!string.IsNullOrWhiteSpace(normalizedName) && normalizedMessage.Contains(normalizedName))
            {
                return true;
            }

            if (keywords.Count == 0)
            {
                return false;
            }

            if (keywords.Count <= 2)
            {
                return bestScore >= keywords.Count;
            }

            return bestScore >= (keywords.Count - 1);
        }

        private static string FormatSingleProductResponse(ProductExtended product, string webInfo)
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(product.Name))
            {
                lines.Add(product.Name);
            }

            var details = new List<string>();
            if (!string.IsNullOrWhiteSpace(product.Brand))
            {
                details.Add(product.Brand);
            }
            if (!string.IsNullOrWhiteSpace(product.CategoryName))
            {
                details.Add(product.CategoryName);
            }
            if (product.BasePrice > 0)
            {
                details.Add(product.BasePrice.ToString("0.##", CultureInfo.InvariantCulture) + " DH");
            }

            if (details.Count > 0)
            {
                lines.Add(string.Join(" | ", details));
            }

            if (!string.IsNullOrWhiteSpace(product.Description))
            {
                string desc = product.Description.Trim();
                if (desc.Length > MaxDescriptionLength)
                {
                    desc = desc.Substring(0, MaxDescriptionLength) + "...";
                }
                lines.Add(desc);
            }

            if (!string.IsNullOrWhiteSpace(webInfo))
            {
                lines.Add("Infos web:");
                lines.Add(webInfo);
            }

            lines.Add("Si tu veux un filtre (prix, marque, categorie), dis-le.");
            return string.Join("\n", lines);
        }

        private static string FormatProductListResponse(List<ProductExtended> products)
        {
            if (products == null || products.Count == 0)
            {
                return "Desole, aucun produit correspondant dans le catalogue.";
            }

            var lines = new List<string> { "Voici des produits disponibles:" };
            foreach (var product in products)
            {
                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(product.Name))
                {
                    parts.Add(product.Name);
                }

                if (!string.IsNullOrWhiteSpace(product.Brand))
                {
                    parts.Add(product.Brand);
                }

                if (!string.IsNullOrWhiteSpace(product.CategoryName))
                {
                    parts.Add(product.CategoryName);
                }

                if (product.BasePrice > 0)
                {
                    parts.Add(product.BasePrice.ToString("0.##", CultureInfo.InvariantCulture) + " DH");
                }

                if (parts.Count > 0)
                {
                    lines.Add("- " + string.Join(" | ", parts));
                }
            }

            lines.Add("Si tu veux un filtre (prix, marque, categorie), dis-le.");
            return string.Join("\n", lines);
        }

        private static string GetWebInfoForProduct(ProductExtended product)
        {
            if (!IsWebSearchEnabled())
            {
                return string.Empty;
            }

            string apiKey = ConfigurationManager.AppSettings["WebSearch:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return string.Empty;
            }

            string provider = ConfigurationManager.AppSettings["WebSearch:Provider"] ?? "SerpApi";
            string endpoint = ConfigurationManager.AppSettings["WebSearch:Endpoint"] ??
                              "https://serpapi.com/search.json";
            int maxResults = GetIntSetting("WebSearch:MaxResults", WebSearchSnippetLimit);

            string query = product.Name;
            if (!string.IsNullOrWhiteSpace(product.Brand))
            {
                query += " " + product.Brand;
            }

            try
            {
                if (provider.Equals("SerpApi", StringComparison.OrdinalIgnoreCase))
                {
                    return FetchSerpApiSnippets(endpoint, apiKey, query, maxResults);
                }
            }
            catch
            {
                return string.Empty;
            }

            return string.Empty;
        }

        private static string FetchSerpApiSnippets(string endpoint, string apiKey, string query, int maxResults)
        {
            if (maxResults <= 0)
            {
                maxResults = WebSearchSnippetLimit;
            }

            string url = endpoint + "?q=" + HttpUtility.UrlEncode(query) +
                         "&hl=fr&gl=ma&num=" + maxResults +
                         "&api_key=" + HttpUtility.UrlEncode(apiKey);

            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("User-Agent", "E-comma");
                var response = http.GetAsync(url).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    return string.Empty;
                }

                var serializer = new JavaScriptSerializer();
                var parsed = serializer.Deserialize<Dictionary<string, object>>(responseBody);
                object resultsObj;
                if (parsed == null || !parsed.TryGetValue("organic_results", out resultsObj))
                {
                    return string.Empty;
                }

                var snippets = new List<string>();
                var resultItems = resultsObj as IEnumerable;
                if (resultItems == null)
                {
                    return string.Empty;
                }

                foreach (var item in resultItems)
                {
                    var result = item as Dictionary<string, object>;
                    if (result == null)
                    {
                        continue;
                    }

                    string title = result.ContainsKey("title") ? result["title"] as string : null;
                    string snippet = result.ContainsKey("snippet") ? result["snippet"] as string : null;
                    string link = result.ContainsKey("link") ? result["link"] as string : null;

                    if (string.IsNullOrWhiteSpace(snippet))
                    {
                        continue;
                    }

                    string line = string.Empty;
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        line += title.Trim() + " - ";
                    }

                    line += snippet.Trim();

                    if (!string.IsNullOrWhiteSpace(link))
                    {
                        line += " (" + link.Trim() + ")";
                    }

                    snippets.Add(line);

                    if (snippets.Count >= maxResults)
                    {
                        break;
                    }
                }

                return snippets.Count > 0 ? string.Join("\n", snippets) : string.Empty;
            }
        }

        private static bool IsWebSearchEnabled()
        {
            string enabled = ConfigurationManager.AppSettings["WebSearch:Enabled"];
            return string.Equals(enabled, "true", StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(normalized.Length);

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        private static int GetIntSetting(string key, int defaultValue)
        {
            string value = ConfigurationManager.AppSettings[key];
            int result;
            return int.TryParse(value, out result) ? result : defaultValue;
        }

        private static List<string> ParseModelCandidates(string modelSetting)
        {
            var candidates = (modelSetting ?? string.Empty)
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(m => m.Trim())
                .Where(m => m.Length > 0)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (candidates.Count == 0)
            {
                candidates.Add("gemini-1.5-flash");
            }

            return candidates;
        }

        private static bool IsModelUnavailableError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            string lower = message.ToLowerInvariant();
            return lower.Contains("(404)") ||
                   lower.Contains("not found") ||
                   lower.Contains("not supported for generatecontent");
        }

        private static string ExtractErrorMessage(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                return null;
            }

            var serializer = new JavaScriptSerializer();
            try
            {
                var parsed = serializer.Deserialize<Dictionary<string, object>>(responseBody);
                if (parsed != null && parsed.ContainsKey("error"))
                {
                    var errorObj = parsed["error"] as Dictionary<string, object>;
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        return errorObj["message"] as string;
                    }
                }
            }
            catch
            {
            }

            if (responseBody.Length > 200)
            {
                return responseBody.Substring(0, 200);
            }

            return responseBody;
        }

        private static void WriteError(HttpContext context, string message)
        {
            context.Response.StatusCode = 400;
            context.Response.Write("{\"error\":\"" + HttpUtility.JavaScriptStringEncode(message) + "\"}");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }

    public class ChatRequest
    {
        public List<ChatMessage> Messages { get; set; }
    }

    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    public class GeminiResponse
    {
        public List<GeminiCandidate> Candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent Content { get; set; }
    }

    public class GeminiContent
    {
        public List<GeminiPart> Parts { get; set; }
    }

    public class GeminiPart
    {
        public string Text { get; set; }
    }
}
