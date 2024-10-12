﻿var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller("UserController", ["$scope", "$http", "$window", function ($scope, $http, $window) {

    $scope.user = {};
    $scope.errors = {};
    $scope.token = "";

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

        function loginUserSuccessCallback(response) {
            $scope.token = response.data.token;
            console.log(response.data.token);
            $http.defaults.headers.common['Authorization'] = 'Bearer ' + $scope.token;

            $http.get('/user/getLoginPartialView')
                .then(getLoginPartialViewSuccessCallback, getLoginPartialViewErrorCallback);

            function getLoginPartialViewSuccessCallback(response) {
                myEl = angular.element(document.querySelector('#login'));
                myEl.html(response.data);
            }

            function getLoginPartialViewErrorCallback(response) {
                console.log(response);
            }

            $http.get('/DayExpenses')
                .then(getDataSuccessCallback, getDataErrorCallback);         

            function getDataSuccessCallback(response) {
                myEl = angular.element(document.querySelector('#bodyContent'));
                myEl.html(response.data);
            }

            function getDataErrorCallback(response) {
                console.log(response);
            }
        }

        function loginUserErrorCallback(response) {
            console.log(response.token);
            $scope.errors = response.data;
        }
    }
}])