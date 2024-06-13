$(function () {
    $("#createCheckButton").on("click", function () {
        $("#staticBackdrop").modal("hide");
        
        var payerDropdown = document.getElementById("payerDropdown");
        var selectedPayer;
        for (var i = 0; i < payerDropdown.length; i++) {
            if (payerDropdown.options[i].selected) {
                selectedPayer = payerDropdown.options[i].value;
                break;
            }                
        }

        $.ajax({
            url: `/Checks/Create?dayexpensesid=${dayExpensesId}`,
            data: {
                Location: $("#checkLocation").val(), Payer: selectedPayer,
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