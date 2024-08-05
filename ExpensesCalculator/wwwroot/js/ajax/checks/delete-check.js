$(function () {
    $("#deleteCheckButton").on("click", function () {
        $("#staticBackdrop").modal("hide");

        $.ajax({
            url: `/Checks/Delete/${checkId}?dayexpensesid=${dayExpensesId}`,
            data: {
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                $("#checkList").html(result);
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    });
});