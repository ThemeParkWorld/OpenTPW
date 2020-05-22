var port = 42069;
var socket;
var authenticated = false;

var packetTypes = {
    Handshake: 0,
    Input: 1,
    Response: 2,
    InputInProgress: 3,
    Suggestions: 4,
    RequestAuth: 5,
    Authenticate: 6,
    RequestLogHistory: 7,
    LogHistory: 8,

    Error: 255
};

function sendObject(obj)
{
    console.log("sent packet of type " + obj.type);
    socket.send(JSON.stringify(obj));
}

function sendHandshake()
{
    var packet = {
        type: packetTypes.Handshake
    }
    sendObject(packet);
}

function sendInput(input)
{
    var packet = {
        type: packetTypes.Input,
        data: {
            input: input
        }
    }
    sendObject(packet);
}

function sendInputInProgress(input)
{
    if (input.indexOf(" ") >= 0)
        input = input.substring(0, input.indexOf(" "));
    
    var packet = {
        type: packetTypes.InputInProgress,
        data: {
            input: input
        }
    }
    sendObject(packet);
}

function sendAuthentication()
{
    var password = prompt("Enter password");
    var packet = {
        type: packetTypes.Authenticate,
        data: {
            password: password
        }
    }
    sendObject(packet);
}

function sendLogHistoryRequest()
{
    var packet = {
        type: packetTypes.RequestLogHistory,
        data: { }
    }
    sendObject(packet);
}

function handleOpen()
{
    console.log("Connected");
    sendHandshake();
    sendLogHistoryRequest();
    showConnectionMessage(false);
}

function handleClose()
{
    console.log("Connection closed");
    showConnectionMessage(true);
}

function handleMessage(e)
{
    e.data.text().then((res) => {
        var packet = JSON.parse(res);
        handlePacket(packet);
    });
}

function handlePacket(packet)
{
    console.log("Received packet of type " + packet.type);
    switch (packet.type)
    {
        case packetTypes.RequestAuth:
            sendAuthentication();
            sendLogHistoryRequest();
            break;
        case packetTypes.Response:
            writeLogString(packet);
            break;
        case packetTypes.LogHistory:
            writeLogHistory(packet);
            break;
        case packetTypes.Suggestions:
            if (packet.data.suggestions == null)
            {
                writeSuggestions([]);
            }
            else
            {
                writeSuggestions(JSON.parse(packet.data.suggestions));
            }
            break;
        case packetTypes.Error: // Error
            alert("Error: " + packet.data.errorMessage);
            break;
        default:
            console.log("Unhandled ", res);
            break;
    }
}

function writeLogString(packet)
{
    logMessage(packet.data.timestamp, packet.data.stackTrace, packet.data.str, packet.data.severity);
}

function writeLogHistory(packet)
{
    var entries = JSON.parse(packet.data.entries);
    for (var entry of entries)
    {
        logMessage(entry.timestamp, entry.stackTrace, entry.str, entry.severity);
    }
}

function tryConnect()
{
    console.log("Trying to connect...");
    socket = new WebSocket("ws://127.0.0.1:" + port, [ "ulaidRcon" ]);
    socket.addEventListener("message", handleMessage);
    socket.addEventListener("open", handleOpen);
    socket.addEventListener("close",handleClose);
    socket.addEventListener("error", function(e) {
        console.log("Websocket fucked it (again): ", event);

        // Retry connection
        tryConnect();
    });
}

tryConnect();