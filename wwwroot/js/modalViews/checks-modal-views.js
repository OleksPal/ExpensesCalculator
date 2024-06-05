function showModalForCheck(act, checkId, dayId) {
    $(document).ready(function () {
        if (act == 'Create') {
            $.get(`/Checks/CreateCheck/?dayExpensesId=${dayId}`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
        else {
            $.get(`/Checks/ChangeCheck/${checkId}?dayExpensesId=${dayId}&act=${act.toString()}`, function (data) {
                $('#modal-content').html(data);
                $('#staticBackdropLabel').html(act);
            });
        }
    });
}
