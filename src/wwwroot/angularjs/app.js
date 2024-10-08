var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller("UserController", ["$scope", "$http", "$window", function ($scope, $http, $window) {

    $scope.user = {};
    $scope.errors = {};

    $scope.isEmpty = function (obj) {
        for (var prop in obj) {
            if (obj.hasOwnProperty(prop))
                return false;
        }
        return true;
    };

    $scope.register = function () {
        var registerUserDto = {};
        registerUserDto.email = $scope.user.email;
        registerUserDto.username = $scope.user.username;
        registerUserDto.password = $scope.user.password;

        $http.post("/user/register", registerUserDto)
            .then(registerUserSuccessCallback, registerUserErrorCallback);

        function registerUserSuccessCallback() {
            $window.location.href = '/user/loginWithUsername';
        }

        function registerUserErrorCallback(response) {
            $scope.errors = response.data;
        }      
    }

    $scope.loginWithUsername = function () {
        var loginUserDto = {};
        loginUserDto.username = $scope.user.username;
        loginUserDto.password = $scope.user.password;

        $http.post("/user/loginWithUsername", loginUserDto)
            .then(loginUserSuccessCallback, loginUserErrorCallback);

        function loginUserSuccessCallback() {
            $window.location.href = '/DayExpenses';
        }

        function loginUserErrorCallback(response) {
            $scope.errors = response.data;
        }
    }
}])