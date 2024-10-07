var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller("UserController", ["$scope", "$http", "$window", function ($scope, $http, $window) {

    $scope.user = {};
    $scope.errors = {};

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
}])