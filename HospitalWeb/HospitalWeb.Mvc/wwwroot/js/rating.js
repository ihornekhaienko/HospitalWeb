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
