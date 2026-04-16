using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace E_comma.Handlers
{
    public class GeminiModels : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetNoStore();

            if (!context.Request.IsLocal)
            {
                context.Response.StatusCode = 403;
                context.Response.Write("{\"error\":\"Forbidden\"}");
                return;
            }

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

            string listUrl = BuildListEndpoint(apiUrl, apiKey);

            try
            {
                string responseBody = GetModels(listUrl);
                WriteModelResponse(context, responseBody);
            }
            catch (Exception ex)
            {
                WriteError(context, "Gemini models error: " + ex.Message);
            }
        }

        private static string BuildListEndpoint(string apiUrl, string apiKey)
        {
            string endpoint = string.IsNullOrWhiteSpace(apiUrl)
                ? "https://generativelanguage.googleapis.com/v1beta/models"
                : apiUrl.Trim();

            int generateIndex = endpoint.IndexOf(":generateContent", StringComparison.OrdinalIgnoreCase);
            if (generateIndex >= 0)
            {
                endpoint = endpoint.Substring(0, generateIndex);
            }

            if (endpoint.EndsWith("/models/", StringComparison.OrdinalIgnoreCase))
            {
                endpoint = endpoint.Substring(0, endpoint.Length - 1);
            }
            else if (!endpoint.EndsWith("/models", StringComparison.OrdinalIgnoreCase))
            {
                endpoint = endpoint.TrimEnd('/') + "/models";
            }

            string separator = endpoint.Contains("?") ? "&" : "?";
            endpoint += separator + "key=" + HttpUtility.UrlEncode(apiKey);
            return endpoint;
        }

        private static string GetModels(string listUrl)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = "E-comma";

                try
                {
                    return client.DownloadString(listUrl);
                }
                catch (WebException ex)
                {
                    var response = ex.Response as HttpWebResponse;
                    string responseBody = null;

                    if (response != null)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    responseBody = reader.ReadToEnd();
                                }
                            }
                        }

                        string errorDetails = ExtractErrorMessage(responseBody);
                        string message = "API request failed (" + (int)response.StatusCode + ")";
                        if (!string.IsNullOrWhiteSpace(errorDetails))
                        {
                            message += ": " + errorDetails;
                        }
                        throw new InvalidOperationException(message);
                    }

                    throw new InvalidOperationException("API request failed");
                }
            }
        }

        private static void WriteModelResponse(HttpContext context, string responseBody)
        {
            var serializer = new JavaScriptSerializer();
            var parsed = serializer.Deserialize<Dictionary<string, object>>(responseBody);
            var models = new List<Dictionary<string, object>>();
            var generateContentModels = new List<string>();

            object modelsObj;
            if (parsed != null && parsed.TryGetValue("models", out modelsObj))
            {
                IEnumerable modelItems = modelsObj as IEnumerable;
                if (modelItems != null)
                {
                    foreach (var item in modelItems)
                    {
                        var modelDict = item as Dictionary<string, object>;
                        if (modelDict == null)
                        {
                            continue;
                        }

                        string name = modelDict.ContainsKey("name") ? modelDict["name"] as string : null;
                        var methods = new List<string>();

                        if (modelDict.ContainsKey("supportedGenerationMethods"))
                        {
                            var methodsObj = modelDict["supportedGenerationMethods"];
                            IEnumerable methodItems = methodsObj as IEnumerable;
                            if (methodItems != null)
                            {
                                foreach (var method in methodItems)
                                {
                                    string methodName = method as string;
                                    if (!string.IsNullOrWhiteSpace(methodName))
                                    {
                                        methods.Add(methodName);
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            models.Add(new Dictionary<string, object>
                            {
                                { "name", name },
                                { "methods", methods }
                            });

                            if (methods.Any(m => string.Equals(m, "generateContent", StringComparison.OrdinalIgnoreCase)))
                            {
                                generateContentModels.Add(name);
                            }
                        }
                    }
                }
            }

            var payload = new
            {
                models = models,
                generateContentModels = generateContentModels
            };

            context.Response.Write(serializer.Serialize(payload));
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
}
