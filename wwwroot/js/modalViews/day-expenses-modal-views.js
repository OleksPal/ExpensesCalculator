function showModalForDayExpensesCreate() {
    $(document).ready(function () {
        $.get(`/DayExpenses/CreateDayExpenses/`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForDayExpensesEdit(dayId) {
    $(document).ready(function () {
        $.get(`/DayExpenses/EditDayExpenses/${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForDayExpensesDelete(dayId) {
    $(document).ready(function () {
        $.get(`/DayExpenses/DeleteDayExpenses/${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}