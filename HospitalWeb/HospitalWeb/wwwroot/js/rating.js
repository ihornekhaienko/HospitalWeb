const ratingHubConnection = new signalR.HubConnectionBuilder()
    .withUrl('/RatingHub')
    .build();

function changeRating(stars, author, target) {
    try {
        notificationHubConnection.invoke("ChangeRating", stars, author, target);
    } catch (err) {
        console.log(err.message);
    }
}

notificationHubConnection.on("ChangeRating", function (rating) {
    try {
        console.log(rating);
    } catch (err) {
        console.log(err.message);
    }
});

ratingHubConnection.start();
