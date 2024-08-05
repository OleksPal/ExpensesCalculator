$(function () {
    $("#shareExpensesButton").on("click", function () {
        $("#statusString").text("");
        if (currentUsersName === $("#newUserWithAccess").val()) {
            $("#statusString").text("This user already has access!");
        }
        else {
            $.ajax({
                url: `/DayExpenses/Share/${dayExpensesId}`,
                data: {
                    NewUserWithAccess: $("#newUserWithAccess").val(),
                    __RequestVerificationToken: $(token).val()
                },
                type: "Post",
                success: function (result) {
                    $("#statusString").text(result);
                },
                error: function (result) {
                    alert(result.responseText);
                }
            });
        }        
    });
});