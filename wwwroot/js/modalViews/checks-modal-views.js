function showModalForCheckCreate(dayId) {
    $(document).ready(function () {
        $.get(`/Checks/CreateCheck/?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForCheckEdit(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Checks/EditCheck/${checkId}?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForCheckDelete(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Checks/DeleteCheck/${checkId}?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}
