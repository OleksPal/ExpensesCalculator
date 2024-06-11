$(function () {
    $("#deleteButton").on("click", function () {
        $("#staticBackdrop").modal("hide");

        $.ajax({
            url: `/Items/Delete/${itemId}?checkid=${checkId}&dayexpensesid=${dayExpensesId}`,
            data: {
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                $(`#check-${checkId}-Items`).html(result);
                $(`#check-${checkId}`).collapse('show');
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    });
});