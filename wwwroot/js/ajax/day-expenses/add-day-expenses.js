$(function () {
    $("#createDayExpensesButton").on("click", function () {
        $.ajax({
            url: `/DayExpenses/Create`,
            data: {
                Date: $("#date").val(), ParticipantsList: $("#participants").val(),
                PeopleWithAccessList: currentUsersName,
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                $('#modal-content').html(result);         
            },
            error: function (result) {
                $('#modal-content').html(data);
            }
        });      
    });
});