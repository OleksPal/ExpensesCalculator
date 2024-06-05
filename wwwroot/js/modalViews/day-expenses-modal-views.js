function showModalForDayExpenses(act, dayId) {
    $(document).ready(function () {
        if (act == 'Create') {
            $.get(`/DayExpenses/CreateDayExpenses/`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
        else {
            $.get(`/DayExpenses/ChangeDayExpenses/${dayId}?act=${act.toString()}`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
    });
}