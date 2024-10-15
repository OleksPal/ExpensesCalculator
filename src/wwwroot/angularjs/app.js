var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', function ($scope) {
    $http.get('/DayExpenses/GetAllDays')
        .then(getAllDaysSuccessfulCallback, getAllDaysErrorCallback);

    function getAllDaysSuccessfulCallback(response) {
        $scope.days = response.data;
    }
    function getAllDaysErrorCallback(error) {
        console.log(error);
    }
}])