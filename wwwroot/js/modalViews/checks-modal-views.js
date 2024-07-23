function showModalForCheckCreate(dayId) {
    $(document).ready(function () {
        $.get(`/Checks/CreateCheck/?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForCheckEdit(checkId) {
    $(document).ready(function () {
        $.get(`/Checks/EditCheck/${checkId}`, function (data) {
            $('#modal-content').html(data);            
        });
    });
}

function showModalForCheckDelete(checkId) {
    $(document).ready(function () {
        $.get(`/Checks/DeleteCheck/${checkId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}
