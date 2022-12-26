const ratingHubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/RatingHub')
    .build();

function changeRating(stars, author, target) {
    try {
        ratingHubConnection.invoke("ChangeRating", stars, author, target);
    } catch (err) {
        console.log(err.message);
    }
}

ratingHubConnection.on("ChangeRating", function (rating, target) {
    try {
        let span = document.getElementById(`rating-overall-${target}`);
        span.innerHTML = rating;
    } catch (err) {
        console.log(err.message);
    }
});

ratingHubConnection.start();

const notificationHubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/NotificationHub')
    .build();

function increment() {
    try {
        let counter = document.getElementById('notifications-count');
        let text = counter.textContent;

        if (text) {
            counter.textContent = +text + 1;
        }
        else {
            counter.appendChild(document.createTextNode('1'));
        }
    } catch (err) {
        console.log(err.message);
    }
}

function decrement() {
    try {
        let counter = document.getElementById('notifications-count');
        let text = counter.textContent;

        if (+text == 1) {
            counter.textContent = null;
        }
        else {
            counter.textContent = +text - 1;
        }
    } catch (err) {
        console.log(err.message);
    }
}

function removeNotification(e) {
    try {
        let notifications = document.getElementById("notifications");
        let firstElem = notifications.firstChild;

        if (firstElem) {
            notifications.removeChild(firstElem);
            decrement();
        }

        e.stopPropagation();
        e.preventDefault();
    }
    catch (err) {
        console.log(err.message);
    }
}

function updatePopover() {
    try {
        const list = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        list.map((el) => {
            let opts = {
                animation: false,
            }
            if (el.hasAttribute('data-bs-content-id')) {
                opts.content = document.getElementById(el.getAttribute('data-bs-content-id')).innerHTML;
                opts.html = true;
            }
            new bootstrap.Popover(el, opts);
        });
    } catch (err) {
        console.log(err.message);
    }
}

function updatePageLink() {
    let pagination = document.getElementsByClassName('pagination')[0];
    console.log(pagination);
    let page = pagination.children.length + 1;
    console.log(pagination);
    console.log(page);
    if (page > 2) {
        return;
    }

    let href = document.URL.split('?')[0] + '?page=' + page;

    let pageLink = document.createElement('a');
    pageLink.classList.add('page-link');
    pageLink.setAttribute('href', href);
    pageLink.innerHTML = page;

    let pageItem = document.createElement('li');
    pageItem.classList.add('page-item');
    pageItem.appendChild(pageLink);

    pagination.classList.remove('d-none');
    pagination.appendChild(pageItem);
}

function createNotificationDiv(topic, message, alertType = 'alert-info') {
    let div = document.createElement('div');
    div.classList.add('alert');
    div.classList.add(alertType);
    div.classList.add('p-1');
    div.classList.add('mb-1');

    let header = document.createElement('h6');
    header.classList.add('alert-heading');
    header.appendChild(document.createTextNode(topic));
    div.appendChild(header);

    let text = document.createElement('p');
    text.classList.add('m-0');
    text.appendChild(document.createTextNode(message));
    div.appendChild(text);

    return div;
}

function notifySignUp(receiver, topic, message) {
    try {
        notificationHubConnection.invoke("NotifySignUp", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

function getNotificationType(type) {
    switch (type) {
        case 0:
            return 'alert-secondary';
        case 1:
            return 'alert-danger';
        case 2:
            return 'alert-success';
        case 3:
            return 'alert-primary';
        default:
            return 'alert-info';
    }
}

function loadLatest() {
    $.ajax({
        type: "POST",
        url: '/Home/LoadLatestNotifications',
        dataType: "json",
        contentType: "application/json",
        success: function (notifications) {
            if (notifications == null) {
                return;
            }
            let popover = document.getElementById("notifications");
            popover.innerHTML = '';
            let counter = document.getElementById('notifications-count');
            counter.innerHTML = '';

            for (let notification of notifications) {
                let type = getNotificationType(notification.type);
                let div = createNotificationDiv(notification.topic, notification.message, type);

                popover.appendChild(div);
                increment();
            }

            updatePopover();
        },
        error: function (err) {
            console.log(err);
        }
    });
}

notificationHubConnection.on("NotifySignUp", function (topic, message) {
    try {
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-primary'), popover.firstChild);

        if (popover.children.length > 5) {
            np.removeChild(np.lastElementChild);
            updatePageLink();
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-primary'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
                updatePageLink();
            }
        }

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

function notifyCancel(receiver, topic, message) {
    try {
        notificationHubConnection.invoke("NotifyCancel", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

notificationHubConnection.on("NotifyCancel", function (topic, message) {
    try {
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-danger'), popover.firstChild);

        if (popover.children.length > 5) {
            np.removeChild(np.lastElementChild);
            updatePageLink();
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-danger'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
                updatePageLink();
            }
        }

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

function notifyFill(receiver, topic, message) {
    try {
        notificationHubConnection.invoke("NotifyFill", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

notificationHubConnection.on("NotifyFill", function (topic, message) {
    try {
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-success'), popover.firstChild);

        if (popover.children.length > 5) {
            np.removeChild(np.lastElementChild);
            updatePageLink();
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-success'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
                updatePageLink();
            }
        }

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

function read(id) {
    try {
        $.ajax({
            url: '/Manage/ReadNotification',
            data: { "id": id.toString() },
            contentType: "application/json",
            success: function (response) {
                let div = document.getElementById('alert-' + id);
                console.log(div);
                div.classList.remove('alert-success');
                div.classList.remove('alert-danger');
                div.classList.remove('alert-primary');
                div.classList.add('alert-secondary');

                loadLatest();
            },
            error: function (err) {
                console.log(err.responseText);
            }
        });
    } catch (err) {
        console.log(err.message);
    }
}

$(document).ready(function () {
    loadLatest();
});

notificationHubConnection.start();

$('#activator').click(function () {
    $('#uploader').click();
});

$('#uploader').change(function () {
    this.form.submit();
});

function confirmSignUp(message) {
    return confirm(message);
}

function updateAppoitmentsStates() {
    $.ajax({
        url: 'https://localhost:7271/jobs/updateStates',
        type: "POST",
        crossDomain: true,
        headers: {
            "accept": "application/json",
            "Access-Control-Allow-Origin": "*"
        },
        success: function (response) {
            console.log('states update');
        },
        error: function (err) {
            console.log('states update failed');
        }
    });
}

updateAppoitmentsStates();
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
    incomeDate.appendChild(document.createTextNode(datetime.toLocaleString('uk-UA')));

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
    outDate.appendChild(document.createTextNode(datetime.toLocaleString('uk-UA')));

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
    cardDate.appendChild(document.createTextNode(datetime.toLocaleString('uk-UA')));

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
    card.id = 'chat-' + userId;
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
        let chat = $(`#char-${userId}`);
        let chatList = $('#chat-list');
        chatList.remove(chat);
        chat = createChat(user, filtered[0].fullName, datetime, message);
        chatList.prepend(chat);
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
        if ($('#chat-user-id').val() == user) {
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