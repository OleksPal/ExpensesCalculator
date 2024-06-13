$(function () {
    $("#editButton").on("click", function () {
        $("#staticBackdrop").modal("hide");

        var userSelect = document.getElementById("Subjects_dropdown");
        var users = [];
        for (var i = 0; i < userSelect.length; i++) {
            if (userSelect.options[i].selected)
                users.push(userSelect.options[i].value);
        }

        $.ajax({
            url: `/Items/Edit/${itemId}?checkid=${checkId}&dayexpensesid=${dayExpensesId}`,
            data: {
                Name: $("#name").val(), Description: $("#description").val(), Price: $("#price").val(), Users: users,
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                $(`#check-${checkId}-Items`).html(result);
                $(`#check-${checkId}-Sum`).text($(`#check-${checkId}-NewSum`).val());
                $(`#check-${checkId}`).collapse('show');
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    });
});