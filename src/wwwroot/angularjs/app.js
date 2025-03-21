var expensesCalculatorApp = angular.module('expensesCalculatorApp', []);

expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', '$http', '$filter', '$compile', '$window',
    function ($scope, $http, $filter, $compile, $window) {

    $http.get('/DayExpenses/GetAllDays')
        .then(getAllDaysSuccessfulCallback, getAllDaysErrorCallback);

    function getAllDaysSuccessfulCallback(response) {
        $scope.days = response.data;
        $scope.filterPagedDays();
    }
    function getAllDaysErrorCallback(error) {
        console.log(error);
        }

    angular.element(document).ready(function () {
        
        // Create DayExpenses
        $scope.showModalForDayExpensesCreate = function () {
            $http.get('/DayExpenses/CreateDayExpenses/').then(function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
                compiledContent = $compile(modalContent)($scope);
            });
        };

        $scope.createDayExpenses = function () {
            var date = $scope.day.date.getUTCFullYear() + '-' +
                ('0' + ($scope.day.date.getUTCMonth() + 1)).slice(-2) + '-' +
                ('0' + $scope.day.date.getUTCDate()).slice(-2);
            var participantsList = $scope.day.participantList;
            var peopleWithAccessList = document.querySelector('input[name="currentUserName"]').value;
            var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            var params = "Date=" + encodeURIComponent(date) +
                "&ParticipantsList=" + encodeURIComponent(participantsList) +
                "&PeopleWithAccessList=" + encodeURIComponent(peopleWithAccessList);

            $http.post(`/DayExpenses/Create`, params, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',  // Set content type for form data
                    'RequestVerificationToken': token  // Anti-forgery token
                }
            })
                .then(function (response) {
                    // Check if response contains div with class modal-body
                    if (response.data.indexOf("<div class=\"modal-body\">") >= 0) {
                        modalContent = angular.element(document.querySelector('#modal-content'));
                        modalContent.html(response.data);
                    }
                    else {
                        $scope.days.push({
                            totalSum: 0,
                            dayExpenses: {
                                date: $scope.day.date.getUTCFullYear() + '-' +
                                ('0' + ($scope.day.date.getUTCMonth() + 1)).slice(-2) + '-' +
                                ('0' + $scope.day.date.getUTCDate()).slice(-2),
                                participantsList: $scope.day.participantList,
                                peopleWithAccessList: peopleWithAccessList = document.querySelector('input[name="currentUserName"]').value
                            }
                        });
                        $('#staticBackdrop').modal('hide');
                        console.log($scope.days);
                    }
            });
        };
    });


    // Share DayExpenses
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
        if ($scope.searchText == undefined || $scope.searchText === "") {
            return true;            
        }

        var dayDate = $filter('date')(day.dayExpenses.date, 'mediumDate').toLowerCase();
        var searchTextLower = $scope.searchText.toLowerCase();

        if (dayDate.indexOf(searchTextLower) !== -1) {
            return true;
        }

        var participantsText = (day.dayExpenses.participantsList.length.toString() + ' people');
        if (participantsText.indexOf($scope.searchText) !== -1) {
            return true;
        }

        if (day.totalSum.toFixed(2).toString().indexOf($scope.searchText) !== -1) {
            return true;
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

    $scope.getCollapseIcon = function (icon) {
        var isCollapsed = angular.element(icon.currentTarget).hasClass('collapsed');

        if (isCollapsed) {
            angular.element(icon.currentTarget).removeClass('bi-dash-square-fill');
            angular.element(icon.currentTarget).addClass('bi-plus-square-fill');
        }
        else {
            angular.element(icon.currentTarget).removeClass('bi-plus-square-fill');
            angular.element(icon.currentTarget).addClass('bi-dash-square-fill');
        }
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
            },
            pagedItems: [],
            currentPage: 0
        };

        var itemsPerPage = 5;
        for (var i = 0; i < value.Items.length; i++) {
            if (i % itemsPerPage === 0) {
                itemCollection.pagedItems[Math.floor(i / itemsPerPage)] = [value.Items[i]];
            } else {
                itemCollection.pagedItems[Math.floor(i / itemsPerPage)].push(value.Items[i]);
            }
        }

        $scope.itemCollections.push(itemCollection);
    });

    $scope.showModalForItemCreate = function (checkId, dayId) {
        $http.get('/Items/CreateItem?checkId=' + checkId + '&dayExpensesId=' + dayId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    $scope.showModalForItemEdit = function (itemId, dayId) {
        $http.get('/Items/EditItem/' + itemId + '?dayExpensesId=' + dayId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    $scope.showModalForItemDelete = function (itemId, dayId) {
        $http.get('/Items/DeleteItem/' + itemId + '?dayExpensesId=' + dayId).then(
            function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
            }
        );
    }

    $scope.getCheckItems = function (checkId) {
        return $scope.itemCollections.find(x => x.checkId === checkId);
    }

    // Sorting items
    $scope.changeOrder = function (value, itemCollection) {
        var sort = itemCollection.sort;

        if (sort.active == value) {
            sort.descending = !sort.descending;
        }
        else {
            sort.active = value;
            sort.descending = false;
        }

        var filteredItems = $filter('orderBy')(itemCollection.items, sort.active, sort.descending);
        itemCollection.pagedItems = $scope.groupToPages(filteredItems);
    };

    $scope.getIcon = function (value, itemCollection) {
        var sort = itemCollection.sort;

        if (sort.active == value) {
            return sort.descending ? 'bi bi-sort-alpha-down-alt' : 'bi bi-sort-alpha-down';
        }

        return 'bi bi-funnel-fill';
    };

    // Filtering items
    $scope.search = function (itemCollection) {
        var filteredItems = [];

        angular.forEach(itemCollection.items, function (value, key) {
            if ($scope.itemSearchText == undefined) {
                filteredItems.push(value);
            }
            else {
                var itemPrice = $filter('currency')(value.Price, '₴');
                if (value.Name.toLowerCase().indexOf($scope.itemSearchText.toLowerCase()) != -1 ||
                    (value.Description && value.Description.toLowerCase().indexOf($scope.itemSearchText.toLowerCase()) != -1) ||
                    itemPrice.indexOf($scope.itemSearchText) != -1 ||
                    ((value.UsersList.length.toString()) + ' people').indexOf($scope.itemSearchText) != -1) {
                    filteredItems.push(value);
                }
            }
        });

        itemCollection.pagedItems = $scope.groupToPages(filteredItems);
    }

    // Pagination
    $scope.itemsPerPage = 5;

    $scope.groupToPages = function (items) {
        var pagedItems = [];

        for (var i = 0; i < items.length; i++) {
            if (i % $scope.itemsPerPage === 0) {
                pagedItems[Math.floor(i / $scope.itemsPerPage)] = [items[i]];
            } else {
                pagedItems[Math.floor(i / $scope.itemsPerPage)].push(items[i]);
            }
        }

        return pagedItems;
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

    $scope.prevPage = function (itemCollection) {
        if (itemCollection.currentPage > 0) {
            itemCollection.currentPage--;
        }
    };

    $scope.nextPage = function (itemCollection) {
        if (itemCollection.currentPage < itemCollection.pagedItems.length - 1) {
            itemCollection.currentPage = itemCollection.currentPage+1;
        }
    };

    $scope.setPage = function (selectedPage, itemCollection) {
        if (selectedPage != undefined) {
            itemCollection.currentPage = selectedPage;
        }
    };

}])