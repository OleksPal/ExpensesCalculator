$(function () {
    $("#editButton").on("click", function () {
        $("#staticBackdrop").modal("hide");

        var userSelect = document.getElementById("Subjects_dropdown");
        var users = "";
        for (var i = 0; i < userSelect.length; i++) {
            if (userSelect.options[i].selected) {
                if (users.length != 0) {
                    users += ",";
                }
                users += userSelect.options[i].value;
            }
        }

        $.ajax({
            url: `/Items/Edit/${itemId}?checkid=${checkId}&dayexpensesid=${dayExpensesId}`,
            data: {
                Name: $("#name").val(), Description: $("#description").val(), Price: $("#price").val(), UsersList: users,
                CheckId: checkId,
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                // Check if response contains div with class modal-body
                if (result.indexOf("<div class=\"modal-body\">") >= 0) {
                    $('#modal-content').html(result);
                }
                else {
                    $("#staticBackdrop").modal("hide");
                    $(`#check-${checkId}-Items`).html(result);
                    $(`#check-${checkId}-Sum`).text($(`#check-${checkId}-NewSum`).val());
                    $(`#check-${checkId}`).collapse('show');
                } 
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    });
});