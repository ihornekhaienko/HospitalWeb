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
        url: 'https://yigalhospitalapi.azurewebsites.net/jobs/updateStates',
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