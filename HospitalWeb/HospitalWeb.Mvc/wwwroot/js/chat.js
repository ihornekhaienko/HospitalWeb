const supportHubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/SupportHub')
    .build();

const chatListPopup = document.querySelector('.chatlist-popup');
const chatPopup = document.querySelector('.chat-popup');
const chatArea = document.querySelector('.chat-area');

let chatBtn = $('#chat-btn');
if (chatBtn.length) {
    chatBtn.click(function () {
        chatPopup.classList.toggle('show');
    });
}

let allChatsBtn = $('#chat-all-users');
if (allChatsBtn.length) {
    allChatsBtn.click(function () {
        document.getElementById('chat-list').classList.toggle('d-none');
        let elems = document.getElementsByClassName('chat-user');
        for (let elem of elems) {
            elem.classList.toggle('d-none');
        }

        let chat = document.getElementById('chat-user');
        chat.innerHTML = '';
        $('#chat-user-id').val('');
    });
}

function createIncomeMessage(message, datetime) {
    let incomeDate = document.createElement('div');
    incomeDate.classList.add('income-date');
    incomeDate.appendChild(document.createTextNode(datetime.toLocaleString()));

    let msg = document.createElement('div');
    msg.classList.add('msg');
    msg.appendChild(incomeDate);
    msg.appendChild(document.createTextNode(message));

    let incomeMsg = document.createElement('div');
    incomeMsg.classList.add('income-msg');
    incomeMsg.appendChild(msg);
    incomeMsg.classList.add('mb-1');

    return incomeMsg;
}

function createOutcomeMessage(message, datetime) {
    let outDate = document.createElement('div');
    outDate.classList.add('out-date');
    outDate.appendChild(document.createTextNode(datetime.toLocaleString()));

    let msg = document.createElement('div');
    msg.classList.add('my-msg');
    msg.appendChild(outDate);
    msg.appendChild(document.createTextNode(message));

    let outMsg = document.createElement('div');
    outMsg.classList.add('out-msg');
    outMsg.appendChild(msg);

    return outMsg;
}

function createChat(userId, fullName, datetime, message) {
    let input = document.createElement('input');
    input.type = 'hidden';
    input.value = userId;

    let cardTitle = document.createElement('h6');
    cardTitle.classList.add('card-title');
    cardTitle.appendChild(document.createTextNode(fullName));

    let cardDate = document.createElement('p');
    cardDate.classList.add('card-text', 'text-muted', 'text-sm', 'mb-0');
    cardDate.appendChild(document.createTextNode(datetime.toLocaleString()));

    let cardMsg = document.createElement('p');
    cardMsg.classList.add('card-text');
    cardMsg.appendChild(document.createTextNode(message));

    let cardBody = document.createElement('div');
    cardBody.classList.add('card-body');
    cardBody.appendChild(input);
    cardBody.appendChild(cardTitle);
    cardBody.appendChild(cardDate);
    cardBody.appendChild(cardMsg);

    let card = document.createElement('div');
    card.classList.add('card', 'mb-1');
    card.id = `chat-${userId}`;
    card.appendChild(cardBody);

    card.onclick = function() {
        openChat(userId);
    }

    return card;
}

function openChat(userId) {
    let chat = document.getElementById('chat-user');

    $.ajax({
        type: "POST",
        url: '/Chat/GetUserMessages',
        dataType: "json",
        contentType: "application/json",
        data: { userId: userId },
        success: function (messages) {
            for (let msg of messages) {
                if (msg.messageType == 0) {
                    let div = createIncomeMessage(msg.text, new Date(msg.dateTime));
                    chat.appendChild(div);
                } else {
                    let div = createOutcomeMessage(msg.text, new Date(msg.dateTime));
                    chat.appendChild(div);
                }
            }
        },
        error: function (err) {
            console.log(err);
        }
    });

    document.getElementById('chat-list').classList.toggle('d-none');
    let elems = document.getElementsByClassName('chat-user');
    for (let elem of elems) {
        elem.classList.toggle('d-none');
    }
    $('#chat-user-id').val(userId);
}

var chats = []

$(document).ready(function () {
    let chatList = $('#chat-list');
    if (!chatList.length) {
        return;
    }

    $.ajax({
        type: "POST",
        url: '/Chat/GetChats',
        dataType: "json",
        contentType: "application/json",
        success: function (model) {
            chats = model;

            for (let chat of chats) {
                let div = createChat(chat.userId, chat.fullName, new Date(chat.lastMessageDateTime), chat.lastMessage);

                chatList.append(div);
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});

$(document).ready(function () {
    let msgList = $('#msg-list');
    if (!msgList.length) {
        return;
    }

    $.ajax({
        type: "POST",
        url: '/Chat/GetUserMessages',
        dataType: "json",
        contentType: "application/json",
        data: { userId: $('#chat-user-id').val() },
        success: function (messages) {
            for (let msg of messages) {
                if (msg.messageType == 0) {
                    let div = createOutcomeMessage(msg.text, new Date(msg.dateTime));
                    msgList.append(div);
                } else {
                    let div = createIncomeMessage(msg.text, new Date(msg.dateTime));
                    msgList.append(div);
                }
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});

function updateChats(user, message, datetime) {
    let filtered = chats.filter(c => c.userId == user);

    if (filtered.length > 0) {
        document.getElementById(`chat-${user}`).remove();
        let chat = createChat(user, filtered[0].fullName, datetime, message);
        $('#chat-list').prepend(chat);
    }
}

let userSend = $('#user-send');
if (userSend.length) {
    userSend.click(function () {
        let userId = $('#chat-user-id').val();
        let message = $('#message-text').val().trim();
        let datetime = new Date();

        if (message.length == 0) {
            return;
        }

        let outcomeMsg = createOutcomeMessage(message, datetime);
        chatArea.insertAdjacentElement("beforeend", outcomeMsg);
        $('#message-text').val('');
        
        sendMessageToAdmins(userId, message, datetime);
    });
}

function sendMessageToAdmins(user, message, datetime) {
    try {
        supportHubConnection.invoke("SendMessageToAdmins", user, message, datetime.toISOString());
    } catch (err) {
        console.log(err.message);
    }
}

supportHubConnection.on("SendMessageToAdmins", function (user, message, datetime) {
    try {
        datetime = new Date(datetime);

        if ($('#chat-user-id').val() == user) {
            alert('add outcome msg');

            let outcomeMsg = createIncomeMessage(message, datetime);
            $('#chat-user').append(outcomeMsg);
        }

        updateChats(user, message, datetime);
    } catch (err) {
        console.log(err.message);
    }
});

let adminSend = $('#admin-send');
if (adminSend.length) {
    adminSend.click(function () {
        let userId = $('#chat-user-id').val();
        let message = $('#message-text').val().trim();
        let datetime = new Date();

        if (message.length == 0) {
            return;
        }

        $('#message-text').val('');

        sendMessageToUser(userId, message, datetime);
    });
}

function sendMessageToUser(user, message, datetime) {
    try {
        supportHubConnection.invoke("SendMessageToUser", user, message, datetime.toISOString());
    } catch (err) {
        console.log(err.message);
    }
}

supportHubConnection.on("SendMessageToUser", function (user, message, datetime) {
    try {
        datetime = new Date(datetime);

        if ($('#chat-list').length) {
            if ($('#chat-user-id').val() == user) {
                let outcomeMsg = createOutcomeMessage(message, datetime);
                $('#chat-user').append(outcomeMsg);
            }

            updateChats(user, message, datetime);
        }
        else {
            let incomeMsg = createIncomeMessage(message, datetime);
            $('#msg-list').append(incomeMsg);
        }
    } catch (err) {
        console.log(err.message);
    }
});

supportHubConnection.start();