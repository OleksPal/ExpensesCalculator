var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', '$http', function ($scope, $http) {
    
    $http.get('/DayExpenses/GetAllDays')
        .then(getAllDaysSuccessfulCallback, getAllDaysErrorCallback);

    function getAllDaysSuccessfulCallback(response) {
        $scope.days = response.data;
    }
    function getAllDaysErrorCallback(error) {
        console.log(error);
    }

    $scope.showModalForDayExpensesShare = function(dayId) {
        $http.get('/DayExpenses/ShareDayExpenses/' + dayId).then(function (response) {
            modalContent = angular.element(document.querySelector('#modal-content'));
            modalContent.html(response.data);
        });
    };

    $scope.showModalForDayExpensesEdit = function(dayId) {
        $http.get('/DayExpenses/EditDayExpenses/' + dayId).then(function (response) {
            modalContent = angular.element(document.querySelector('#modal-content'));
            modalContent.html(response.data);
        });
    };

    $scope.showModalForDayExpensesDelete = function(dayId) {
        $http.get('/DayExpenses/DeleteDayExpenses/' + dayId).then(function (response) {
            modalContent = angular.element(document.querySelector('#modal-content'));
            modalContent.html(response.data);
        });
    };

    // Sorting days
    $scope.sort = {
        active: '',
        descending: undefined
    };

    $scope.changeOrder = function (value) {
        var sort = $scope.sort;

        if (sort.active == value) {
            sort.descending = !sort.descending;
        }
        else {
            sort.active = value;
            sort.descending = false;
        }
    };

    $scope.getIcon = function (value) {
        var sort = $scope.sort;

        if (sort.active == value) {
            return sort.descending ? 'bi bi-sort-alpha-down-alt' : 'bi bi-sort-alpha-down';
        }

        return 'bi bi-arrow-down-up';
    };

    //$scope.order = '';
    //$scope.changeOrder = function (value) {
    //    if ($scope.order === value) {
    //        $scope.order = '-' + value;
    //    }
    //    else if ($scope.order === '-' + value) {
    //        $scope.order = value;
    //    }
    //    else {
    //        $scope.order = value;
    //    }
    //}
}])