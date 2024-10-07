var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller("UserController", ["$scope", "$http", function ($scope, $http) {

    $scope.user = {};
    $scope.register = function () {
        var registerUserDto = {};
        registerUserDto.email = $scope.user.email;
        registerUserDto.username = $scope.user.username;
        registerUserDto.password = $scope.user.password;

        $http.post("/user/register", registerUserDto)
            .then(registerUserSuccessCallback, registerUserErrorCallback);

        function registerUserSuccessCallback() {

        }

        function registerUserErrorCallback() {

        }      
    }
}])