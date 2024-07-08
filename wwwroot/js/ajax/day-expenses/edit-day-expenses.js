$(function () {
    $("#editDayExpensesButton").on("click", function () {
        $.ajax({
            url: `/DayExpenses/Edit/${$("#Id").val()}`,
            data: {
                Date: $("#date").val(), ParticipantsList: $("#participants").val(),
                PeopleWithAccessList: currentUsersName,
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                // Check if response contains div with class modal-body
                if (result.indexOf("<div class=\"modal-body\">") >= 0)
                    $('#modal-content').html(result);
                else
                    location.reload(true);
            },
            error: function (result) {
                $('#modal-content').html(result);
            }
        });      
    });
});