var consoleWrapper = document.getElementById("console-wrapper");
var consoleInput = document.getElementById("console-input");
var message = document.getElementById("message");
var consoleDiv = document.getElementById("console");
var consoleSuggestions = document.getElementById("console-suggestions");

function showConnectionMessage(shown)
{
    message.style.visibility = shown ? "visible" : "hidden";
}

function inputKeyDown(e)
{
    if (e.keyCode == 13)
    {
        logInput(consoleInput.value);
        consoleInput.value = "";
    }
    
    if (consoleInput.value == "")
    {
        consoleSuggestions.style.visibility = "hidden";
    }
    else
    {
        consoleSuggestions.style.visibility = "visible";
        sendInputInProgress(consoleInput.value);
    }
}

function writeSuggestions(suggestionsList)
{
    consoleSuggestions.innerHTML = "";

    if (suggestionsList.length == 0 || suggestionsList == null)
    {
        consoleSuggestions.innerHTML = `
        <li>
            <span class="console-suggestion-error">No commands found.</span>
        </li>`;
        return;
    }

    for (var suggestion of suggestionsList)
    {
        var template = `
            <li>
                <span class="console-suggestion-command">{{alias}} <span class="console-suggestion-value">{{currentValue}}</span></span>
                <span class="console-suggestion-description">{{description}}</span>
            </li>`;

        if (suggestion.value == undefined)
            suggestion.value = "";
            
        var templateProcessed = template.replace("{{alias}}", suggestion.name).replace("{{description}}", suggestion.description).replace("{{currentValue}}", suggestion.value);


        document.getElementById("console-suggestions").innerHTML += templateProcessed;
    }
}

function writeToConsole(str)
{
    consoleDiv.innerHTML += str;
    consoleWrapper.scrollTo(0, consoleWrapper.scrollHeight);
}

function logMessage(timestamp, stackTrace, str, severity)
{
    var template = `
        <div class="console-message {{severity}}">
            <span class="console-timestamp">{{timestamp}}</span>
            <span class="console-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{severity}}", severity).replace("{{timestamp}}", timestamp).replace("{{str}}", str).replace("{{stackTrace}}", stackTrace);
    writeToConsole(templateProcessed);
}

function logInput(str)
{
    var template = `
        <div class="console-input-message">
            <span class="console-input-message-string">{{str}}</span>
        </div>
    `;

    var templateProcessed = template.replace("{{str}}", "> " + str);
    writeToConsole(templateProcessed);
    sendInput(str);
}