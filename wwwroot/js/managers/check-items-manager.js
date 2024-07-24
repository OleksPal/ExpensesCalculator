function getCheckItemsManager(checkId, dayId) {
    $(document).ready(function () {
        if ($(`#check-${checkId}-Items`).is(':empty')) {
            $.get(`/Checks/GetCheckItemsManager/${checkId}?dayExpensesId=${dayId}`, function (data) {
                $(`#check-${checkId}-Items`).html(data);
                $(`#check-${checkId}`).collapse('show');
            });
        }
        else {
            var attr = $(`#checkItemList-${checkId}`).attr('hidden');

            // For some browsers, `attr` is undefined; for others, `attr` is false. Check for both.
            if (typeof attr !== typeof undefined && attr !== false) {
                // Element has the hidden attribute   
                $(`#checkItemList-${checkId}`).removeAttr('hidden');

                function delayShow() {
                    $(`#check-${checkId}`).collapse('show');
                }

                setTimeout(delayShow, 100);
            }
            else {
                $(`check-${checkId}`).collapse('hide');
 
                function delayHide() {
                    $(`#checkItemList-${checkId}`).attr('hidden', true);
                }

                setTimeout(delayHide, 300);
            }
        }        
    });
}