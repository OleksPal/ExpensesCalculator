var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', '$http', '$filter', function ($scope, $http, $filter) {

    $http.get('/DayExpenses/GetAllDays')
        .then(getAllDaysSuccessfulCallback, getAllDaysErrorCallback);

    function getAllDaysSuccessfulCallback(response) {
        $scope.days = response.data;
        $scope.filterPagedDays();
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

        return 'bi bi-funnel-fill';
    };

    // Filtering days
    $scope.search = function (day) {
        if ($scope.searchText == undefined) {
            return true;            
        }
        else {
            var dayDate = $filter('date')(day.date, 'mediumDate');
            dayDate = dayDate.toLowerCase();
            if (dayDate.indexOf($scope.searchText.toLowerCase()) != -1 ||
                ((day.participantsList.length.toString()) + ' people').indexOf($scope.searchText) != -1)
            {
                return true;
            }
        }

        return false;
    }

    // Pagination
    $scope.daysPerPage = 5;
    $scope.filteredDays = [];
    $scope.pagedDays = [];
    $scope.currentPage = 0;

    $scope.filterPagedDays = function () {
        $scope.filteredDays = $filter('filter')($scope.days, function (day) {
            return $scope.search(day);
        });
        $scope.groupToPages();
    }

    $scope.groupToPages = function () {
        for (var i = 0; i < $scope.filteredDays.length; i++) {
            if (i % $scope.daysPerPage === 0) {
                $scope.pagedDays[Math.floor(i / $scope.daysPerPage)] = [$scope.filteredDays[i]];
            } else {
                $scope.pagedDays[Math.floor(i / $scope.daysPerPage)].push($scope.filteredDays[i]);
            }
        }
    };

    $scope.range = function (start, end) {
        var ret = [];
        if (!end) {
            end = start;
            start = 0;
        }
        for (var i = start; i < end; i++) {
            ret.push(i);
        }
        return ret;
    };

    $scope.prevPage = function () {
        if ($scope.currentPage > 0) {
            $scope.currentPage--;
        }
    };

    $scope.nextPage = function () {
        if ($scope.currentPage < $scope.pagedDays.length - 1) {
            $scope.currentPage++;
        }
    };

    $scope.setPage = function () {
        $scope.currentPage = this.n;
    };
}])