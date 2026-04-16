(function () {
    var launcher = document.getElementById('chatbotLauncher');
    var panel = document.getElementById('chatbotPanel');
    var closeBtn = document.getElementById('chatbotClose');
    var messagesContainer = document.getElementById('chatbotMessages');
    var input = document.getElementById('chatbotInput');
    var sendBtn = document.getElementById('chatbotSend');
    var typingEl = null;
    var history = [];

    if (!launcher || !panel || !messagesContainer || !input || !sendBtn) {
        return;
    }

    function toggleChat(open) {
        if (open) {
            panel.classList.add('is-open');
            input.focus();
        } else {
            panel.classList.remove('is-open');
        }
    }

    launcher.addEventListener('click', function () {
        toggleChat(!panel.classList.contains('is-open'));
    });

    if (closeBtn) {
        closeBtn.addEventListener('click', function () {
            toggleChat(false);
        });
    }

    function appendMessage(role, content) {
        var wrapper = document.createElement('div');
        wrapper.className = 'chatbot-message ' + role;
        wrapper.textContent = content;
        messagesContainer.appendChild(wrapper);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }

    function setTyping(show) {
        if (show) {
            if (!typingEl) {
                typingEl = document.createElement('div');
                typingEl.className = 'chatbot-typing';
                typingEl.textContent = 'E-comma est en train de repondre...';
                messagesContainer.appendChild(typingEl);
                messagesContainer.scrollTop = messagesContainer.scrollHeight;
            }
        } else if (typingEl) {
            messagesContainer.removeChild(typingEl);
            typingEl = null;
        }
    }

    function addToHistory(role, content) {
        history.push({ role: role, content: content });
        if (history.length > 10) {
            history = history.slice(history.length - 10);
        }
    }

    function sendMessage() {
        var text = input.value.trim();
        if (!text) {
            return;
        }

        input.value = '';
        appendMessage('user', text);
        addToHistory('user', text);
        setTyping(true);

        fetch('/Handlers/Chatbot.ashx', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'same-origin',
            body: JSON.stringify({ messages: history })
        })
            .then(function (response) {
                return response.json();
            })
            .then(function (data) {
                setTyping(false);
                if (data && data.error) {
                    appendMessage('bot', data.error);
                    return;
                }
                var reply = data && data.reply ? data.reply : "Desole, je n'ai pas pu repondre.";
                appendMessage('bot', reply);
                addToHistory('assistant', reply);
            })
            .catch(function () {
                setTyping(false);
                appendMessage('bot', "Erreur de connexion. Veuillez reessayer.");
            });
    }

    sendBtn.addEventListener('click', function () {
        sendMessage();
    });

    input.addEventListener('keydown', function (event) {
        if (event.key === 'Enter') {
            event.preventDefault();
            sendMessage();
        }
    });

    appendMessage('bot', "Bonjour, comment puis-je vous aider aujourd'hui ?");
})();
