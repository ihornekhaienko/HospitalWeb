const hubConnection = new signalR.HubConnectionBuilder()
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
        hubConnection.invoke("NotifySignUp", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

hubConnection.on("NotifySignUp", function (topic, message) {
    try {
        document.getElementById("notifications").appendChild(createNotificationDiv(topic, message, 'alert-info'));
        let row = document.createElement('div');
        row.classList.add('row');
        row.appendChild(createNotificationDiv(topic, message, 'alert-info'));
        document.getElementById("notifications-profile").appendChild(row);

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

function notifyCancel(receiver, topic, message) {
    try {
        hubConnection.invoke("NotifyCancel", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

hubConnection.on("NotifyCancel", function (topic, message) {
    try {
        document.getElementById("notifications").appendChild(createNotificationDiv(topic, message, 'alert-danger'));
        let row = document.createElement('div');
        row.classList.add('row');
        row.appendChild(createNotificationDiv(topic, message, 'alert-danger'));
        document.getElementById("notifications-profile").appendChild(row);

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

function notifyFill(receiver, topic, message) {
    try {
        hubConnection.invoke("NotifyFill", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

hubConnection.on("NotifyFill", function (topic, message) {
    try {
        document.getElementById("notifications").appendChild(createNotificationDiv(topic, message, 'alert-success'));
        let row = document.createElement('div');
        row.classList.add('row');
        row.appendChild(createNotificationDiv(topic, message, 'alert-success'));
        document.getElementById("notifications-profile").appendChild(row);

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

hubConnection.start();
