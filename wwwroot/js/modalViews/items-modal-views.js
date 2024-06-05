function showModalForItem(act, itemId, checkId, dayId) {
    $(document).ready(function () {
        if (act == 'Create') {
            $.get(`/Items/CreateItem/?checkId=${checkId}&dayExpensesId=${dayId}`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
        else {
            $.get(`/Items/ChangeItem/${itemId}?checkId=${checkId}&dayExpensesId=${dayId}&act=${act.toString()}`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
    });
}