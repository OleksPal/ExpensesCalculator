﻿var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

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

        $scope.days = $filter('orderBy')($scope.days, sort.active, sort.descending);
        $scope.filterPagedDays();
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
        $scope.pagedDays = [];
        this.n = 0;
        $scope.setPage();

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

expensesCalculatorApp.controller('DayExpensesChecksCtrl', ['$scope', '$http', '$filter', function ($scope, $http, $filter) {
    var dayExpensesId = angular.element(document.querySelector('#dayExpensesId')).val();

    $http.get('/DayExpenses/GetDayExpensesChecks/' + dayExpensesId)
        .then(getChecksSuccessfulCallback, getChecksErrorCallback);

    function getChecksSuccessfulCallback(response) {
        $scope.dayExpenses = response.data;
        $scope.checks = $scope.dayExpenses.Checks;
        $scope.filterPagedChecks();
    }
    function getChecksErrorCallback(error) {
        console.log(error);
    }

    $scope.showModalForCheckCreate = function (dayId) {
        $http.get('/Checks/CreateCheck/?dayExpensesId=' + dayId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    $scope.showModalForCheckEdit = function (checkId) {
        $http.get('/Checks/EditCheck/' + checkId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    $scope.showModalForCheckDelete = function(checkId) {
        $http.get('/Checks/DeleteCheck/' + checkId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    // Sorting checks
    $scope.sort = {
        active: '',
        descending: undefined
    };

    $scope.changeOrder = function (value) {
        var sort = $scope.sort;
        $scope.currentPage = 0;

        if (sort.active == value) {
            sort.descending = !sort.descending;
        }
        else {
            sort.active = value;
            sort.descending = false;
        }

        $scope.checks = $filter('orderBy')($scope.checks, sort.active, sort.descending);
        $scope.filterPagedChecks();
    };


    $scope.getIcon = function (value) {
        var sort = $scope.sort;

        if (sort.active == value) {
            return sort.descending ? 'bi bi-sort-alpha-down-alt' : 'bi bi-sort-alpha-down';
        }

        return 'bi bi-funnel-fill';
    };

    // Filtering checks
    $scope.search = function (check) {
        if ($scope.searchText == undefined) {
            return true;
        }
        else {
            var checkSum = $filter('currency')(check.Sum, '₴');
            if (check.Location.toLowerCase().indexOf($scope.searchText.toLowerCase()) != -1 ||
                checkSum.indexOf($scope.searchText) != -1 ||
                check.Payer.toLowerCase().indexOf($scope.searchText.toLowerCase()) != -1) {
                return true;
            }
        }

        return false;
    }

    // Pagination
    $scope.checksPerPage = 5;
    $scope.filteredChecks = [];
    $scope.pagedChecks = [];
    $scope.currentPage = 0;

    $scope.filterPagedChecks = function () {
        $scope.pagedChecks = [];
        $scope.currentPage = 0;

        $scope.filteredChecks = $filter('filter')($scope.checks, function (check) {
            return $scope.search(check);
        });
        $scope.groupToPages();
    }

    $scope.groupToPages = function () {
        for (var i = 0; i < $scope.filteredChecks.length; i++) {
            if (i % $scope.checksPerPage === 0) {
                $scope.pagedChecks[Math.floor(i / $scope.checksPerPage)] = [$scope.filteredChecks[i]];
            } else {
                $scope.pagedChecks[Math.floor(i / $scope.checksPerPage)].push($scope.filteredChecks[i]);
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
        if ($scope.currentPage < $scope.pagedChecks.length - 1) {
            $scope.currentPage++;
        }
    };

    $scope.setPage = function () {
        if ($scope.selectedPage != undefined) {
            $scope.currentPage = ($scope.selectedPage - 1);
        }
    };
}])

expensesCalculatorApp.controller('ItemsCtrl', ['$scope', '$http', '$filter', function ($scope, $http, $filter) {
    $scope.itemCollections = [];

    angular.forEach($scope.checks, function (value, key) {
        var itemCollection = {
            checkId: value.Id,
            items: value.Items,
            sort: {
                active: '',
                descending: undefined
            }
        };
        $scope.itemCollections.push(itemCollection);
    });    

    $scope.getCheckItems = function (checkId) {
        return $scope.itemCollections.find(x => x.checkId === checkId);
    }

    // Sorting items
    $scope.changeOrder = function (value, checkId) {
        var itemCollection = $scope.getCheckItems(checkId);
        var sort = itemCollection.sort;

        if (sort.active == value) {
            sort.descending = !sort.descending;
        }
        else {
            sort.active = value;
            sort.descending = false;
        }

        itemCollection.items = $filter('orderBy')(itemCollection.items, sort.active, sort.descending);
    };

    $scope.getIcon = function (value, checkId) {
        var itemCollection = $scope.getCheckItems(checkId);
        var sort = itemCollection.sort;

        if (sort.active == value) {
            return sort.descending ? 'bi bi-sort-alpha-down-alt' : 'bi bi-sort-alpha-down';
        }

        return 'bi bi-funnel-fill';
    };

    // Filtering items
    $scope.search = function (item) {
        if ($scope.searchText == undefined) {
            return true;
        }
        else {
            var itemPrice = $filter('currency')(item.Price, '₴');
            if (item.Name.toLowerCase().indexOf($scope.searchText.toLowerCase()) != -1 ||
                item.Description.toLowerCase().indexOf($scope.searchText.toLowerCase()) != -1 ||
                itemPrice.indexOf($scope.searchText) != -1 ||
                ((item.UsersList.length.toString()) + ' people').indexOf($scope.searchText) != -1) {
                return true;
            }
        }

        return false;
    }

    // Pagination
    $scope.itemsPerPage = 5;
    $scope.filteredItems = [];
    $scope.pagedItems = [];
    $scope.currentPage = 0;

    $scope.filterPagedItems = function () {
        $scope.pagedItems = [];
        $scope.currentPage = 0;

        $scope.filteredItems = $filter('filter')($scope.items, function (item) {
            return $scope.search(item);
        });
        $scope.groupToPages();
    }

    $scope.groupToPages = function () {
        for (var i = 0; i < $scope.filteredItems.length; i++) {
            if (i % $scope.itemsPerPage === 0) {
                $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)] = [$scope.filteredItems[i]];
            } else {
                $scope.pagedItems[Math.floor(i / $scope.itemsPerPage)].push($scope.filteredItems[i]);
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
        if ($scope.currentPage < $scope.pagedChecks.length - 1) {
            $scope.currentPage++;
        }
    };

    $scope.setPage = function () {
        if ($scope.selectedPage != undefined) {
            $scope.currentPage = ($scope.selectedPage - 1);
        }
    };

}])