function showModalForItemCreate(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Items/CreateItem/?checkId=${checkId}&dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForItemEdit(itemId, checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Items/EditItem/${itemId}?checkId=${checkId}&dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForItemDelete(itemId, checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Items/DeleteItem/${itemId}?checkId=${checkId}&dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}