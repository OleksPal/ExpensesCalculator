var expensesCalculatorInterceptor = function ($window, $q) {
    return {
        request: function (config) {
            config.headers['Authorization'] = 'Bearer ' + $window.localStorage['jwtToken'];
            return config || $q.when(config);
        }
    };
};

var expensesCalculatorApp = angular.module('expensesCalculatorApp', []).config(function ($httpProvider) {
    $httpProvider.interceptors.push(expensesCalculatorInterceptor);
});

expensesCalculatorApp.controller("UserController", ["$scope", "$http", "$window", "$location", function ($scope, $http, $window, $location) {

    $scope.user = {};
    $scope.errors = {};
    $window.localStorage['jwtToken'] = '';

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
            $window.localStorage['jwtToken'] = response.data.token;
            $http.defaults.headers.common['Authorization'] = 'Bearer ' + $window.localStorage['jwtToken'];

            $http.get('/DayExpenses')
                .then((response) => {
                    document.open();
                    console.log(response);
                    document.write(response.data);
                    document.close();
                });
        }

        function loginUserErrorCallback(response) {
            console.log(response);
            $scope.errors = response.data;
        }
    }
}])