function showModalForCheckCreate(dayId) {
    $(document).ready(function () {
        $.get(`/Checks/CreateCheck/?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForCheckEdit(checkId) {
    alert(checkId);
    $(document).ready(function () {
        $.get(`/Checks/EditCheck/${checkId}`, function (data) {
            $('#modal-content').html(data);            
        });
    });
}z

function showModalForCheckDelete(checkId) {
    $(document).ready(function () {
        $.get(`/Checks/DeleteCheck/${checkId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}
