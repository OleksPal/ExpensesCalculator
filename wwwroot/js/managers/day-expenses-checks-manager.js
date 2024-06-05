function getDayExpensesChecksManager(dayId) {
    $(document).ready(function () {
        if ($(`#day-${dayId}-Checks`).is(':empty')) {
            $.get(`/DayExpenses/GetDayExpensesChecksManager/${dayId}`, function (data) {
                $(`#dayDropper-${dayId}`).html('&#9698;');
                $(`#day-${dayId}-Checks`).html(data);
                $(`#day-${dayId}`).collapse('show');
            });
        }
        else {
            var collapsedRow = document.getElementById(`day-${dayId}`);
            $(`#day-${dayId}`).collapse('hide');
            $(`#dayDropper-${dayId}`).html('&#9655;');

            collapsedRow.addEventListener('hidden.bs.collapse', () => {
                $(`#day-${dayId}-Checks`).empty();
            });
        }
    });
}
