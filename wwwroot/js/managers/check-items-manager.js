function getCheckItemsManager(checkId, dayId) {
    $(document).ready(function () {
        if ($(`#check-${checkId}-Items`).is(':empty')) {
            $.get(`/Checks/GetCheckItemsManager/${checkId}?dayExpensesId=${dayId}`, function (data) {
                $(`#checkDropper-${checkId}`).html('&#9698;');
                $(`#check-${checkId}-Items`).html(data);
                $(`#check-${checkId}`).collapse('show');
            });
        }
        else {
            var collapsedRow = document.getElementById(`check-${checkId}`);
            $(`check-${checkId}`).collapse('hide');
            $(`#checkDropper-${checkId}`).html('&#9655;');

            collapsedRow.addEventListener('hidden.bs.collapse', () => {
                $(`#check-${checkId}-Items`).empty();
            });
        }
    });
}