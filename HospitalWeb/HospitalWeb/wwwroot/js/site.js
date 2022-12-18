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
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-primary'), popover.firstChild);
        if (popover.children.length > 5) {
            popover.removeChild(np.lastElementChild);
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-primary'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
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
        hubConnection.invoke("NotifyCancel", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

hubConnection.on("NotifyCancel", function (topic, message) {
    try {
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-danger'), popover.firstChild);
        if (popover.children.length > 5) {
            popover.removeChild(np.lastElementChild);
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-danger'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
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
        hubConnection.invoke("NotifyFill", receiver, topic, message);
    } catch (err) {
        console.log(err.message);
    }
}

hubConnection.on("NotifyFill", function (topic, message) {
    try {
        let popover = document.getElementById("notifications");
        popover.insertBefore(createNotificationDiv(topic, message, 'alert-success'), popover.firstChild);
        if (popover.children.length > 5) {
            popover.removeChild(np.lastElementChild);
        }

        let np = document.getElementById("notifications-profile");
        if (np) {
            let row = document.createElement('div');
            row.classList.add('row');
            row.appendChild(createNotificationDiv(topic, message, 'alert-success'));
            np.insertBefore(row, np.firstChild);

            if (np.children.length > 5) {
                np.removeChild(np.lastElementChild);
            }
        }

        increment();
        updatePopover();
    } catch (err) {
        console.log(err.message);
    }
});

hubConnection.start();

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