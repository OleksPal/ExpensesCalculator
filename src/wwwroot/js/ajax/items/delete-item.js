$(function () {
    $("#deleteButton").on("click", function () {
        $("#staticBackdrop").modal("hide");

        $.ajax({
            url: `/Items/Delete/${itemId}`,
            data: {
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                location.reload();
                //$(`#check-${checkId}-Items`).html(result);
                //$(`#check-${checkId}-Sum`).text($(`#check-${checkId}-NewSum`).val());
                //$(`#check-${checkId}`).collapse('show');
            },
            error: function (result) {
                alert(result.responseText);
            }
        });
    });
});