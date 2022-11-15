var interval = 1000 * 60 * 60;

function updateAppoitmentsStates() {
    $.ajax({
        url: '@Url.Action("UpdateAppointmentStates", "Home")',
        dataType: "html",
        type: "GET",
        contentType: "application/json",
        success: function (response) {
            console.log('states update');
        },
        error: function (err) {
            console.log('states update failed');
        }
    });
}

setInterval(updateAppoitmentsStates, interval);