function showModalForItemCreate(checkId, dayId) {
    $(document).ready(function () {
        $.get(`/Items/CreateItem/?checkId=${checkId}&dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForItemEdit(itemId, dayId) {
    $(document).ready(function () {
        $.get(`/Items/EditItem/${itemId}?dayExpensesId=${dayId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}

function showModalForItemDelete(itemId) {
    $(document).ready(function () {
        $.get(`/Items/DeleteItem/${itemId}`, function (data) {
            $('#modal-content').html(data);
        });
    });
}
