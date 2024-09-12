$(function () {
    $("#createDayExpensesButton").on("click", function () {
        $.ajax({
            url: `/DayExpenses/Create`,
            data: {
                Date: $("#date").val(), ParticipantList: $("#participants").val(),
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