$(function () {
    $("#createCheckButton").on("click", function () {       
        var payerDropdown = document.getElementById("payerDropdown");
        var selectedPayer;
        for (var i = 0; i < payerDropdown.length; i++) {
            if (payerDropdown.options[i].selected) {
                selectedPayer = payerDropdown.options[i].value;
                break;
            }                
        }

        $.ajax({
            url: `/Checks/Create`,
            data: {
                Location: $("#checkLocation").val(), Payer: selectedPayer,
                DayExpensesId: dayExpensesId,
                __RequestVerificationToken: $(token).val()
            },
            type: "Post",
            success: function (result) {
                // Check if response contains div with class modal-body
                if (result.indexOf("<div class=\"modal-body\">") >= 0)
                    $('#modal-content').html(result);
                else {
                    $("#staticBackdrop").modal("hide");
                    $("#checkList").html(result);
                }                                                
            },
            error: function (result) {
                $('#modal-content').html(result);
            }
        });      
    });
});