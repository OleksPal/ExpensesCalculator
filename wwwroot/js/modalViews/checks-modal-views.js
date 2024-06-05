function showModalForCheckCreate(dayId) {
    $(document).ready(function () {
        $.get(`/Checks/CreateCheck/?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
            $('#staticBackdropLabel').html(act);
        });
    });
}

function showModalForCheckEdit(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Checks/EditCheck/${checkId}?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
            $('#staticBackdropLabel').html(act);
        });
    });
}

function showModalForCheckDelete(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Checks/DeleteCheck/${checkId}?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
            $('#staticBackdropLabel').html(act);
        });
    });
}
