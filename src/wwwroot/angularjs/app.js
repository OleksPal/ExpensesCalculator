var expensesCalculatorApp = angular.module('expensesCalculatorApp', ['ngAnimate']);

expensesCalculatorApp.service('toastService', ['$timeout', function ($timeout) {
    // Initialize toasts array in the service
    this.toasts = [];

    // Function to show toast
    this.showToast = function (type, title, message) {
        var newToast = {
            type: 'bg-' + type,
            title: title,
            message: message,
            time: new Date().toLocaleTimeString(),
            hidden: false,
            id: 'toast-' + this.toasts.length
        };

        // Add the toast to the toasts array
        this.toasts.push(newToast);

        // Use $timeout to ensure DOM is ready
        $timeout(function () {
            var toastElement = document.getElementById(newToast.id);
            if (toastElement) {
                var toast = new bootstrap.Toast(toastElement);
                toast.show();
            }
        }, 100);
    };
}]);


expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', '$http', '$filter', '$compile', 'toastService',
    function ($scope, $http, $filter, $compile, toastService) {

        // Expose the toastService's toasts array to the scope (if you need to access it in the view)
        $scope.toasts = toastService.toasts;

        // Use the showToast method from the toastService
        $scope.showToast = function (type, title, message) {
            toastService.showToast(type, title, message);
        };

        $http.get('/DayExpenses/GetAllDays')
            .then(getAllDaysSuccessfulCallback, getAllDaysErrorCallback);

        function getAllDaysSuccessfulCallback(response) {
            $scope.days = response.data;
            $scope.filterPagedDays();
        }
        function getAllDaysErrorCallback(error) {
            $scope.showToast('danger', 'Fail!', "Error: " + error.config.url + " - " + error.statusText);
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
                var date = ($scope.day && $scope.day.date !== undefined)
                    ? $filter('date')($scope.day.date, 'yyyy-MM-ddTHH:mm:ss')
                    : "None";
                var participantsList = ($scope.day && $scope.day.participantList !== undefined)
                    ? $scope.day.participantList
                    : "";
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
                        if (typeof response.data === 'string' && response.data.indexOf("<div class=\"modal-body\">") >= 0) {
                            modalContent = angular.element(document.querySelector('#modal-content'));
                            modalContent.html(response.data);
                            compiledContent = $compile(modalContent)($scope);                       
                        }
                        else {
                            $scope.day = { date: '', participantList: '' };
                            $('#staticBackdrop').modal('hide');

                            // Move one page forward if added element can`t be displayed on the same page
                            var currentPage = $scope.currentPage;

                            if ($scope.pagedDays[currentPage] === undefined)
                                $scope.pagedDays[currentPage] = [];                            
                                
                            $scope.days.push(response.data);                            

                            $scope.filterPagedDays();

                            if (currentPage === -1 || $scope.pagedDays[currentPage].length === 5)
                                currentPage = $scope.pagedDays.length - 1;

                            $scope.currentPage = currentPage;

                            $scope.showToast('success', 'Success!', 'Day was successfully added.');
                            $scope.triggerAnimation($scope.pagedDays[currentPage].length - 1, 'create');                                     
                        }
                });
            };
        });

        // Variables to track which row should be animated
        $scope.animatedRowIndex = null;
        $scope.animationType = null;

        // Trigger animation for the specific row
        $scope.triggerAnimation = function (index, animationType) {
            $scope.animatedRowIndex = index;
            $scope.animationType = animationType;

            // Reset the animation after the duration
            setTimeout(function () {
                $scope.$apply(function () {
                    $scope.animatedRowIndex = null; 
                    $scope.animationType = null;
                });
            }, 500);
        };

        // Share DayExpenses
        $scope.showModalForDayExpensesShare = function(dayId) {
            $http.get('/DayExpenses/ShareDayExpenses/' + dayId).then(function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
                compiledContent = $compile(modalContent)($scope);
            });
        };

        $scope.shareDayExpenses = function () {
            
            var currentUsersName = document.querySelector('input[name="currentUsersName"]').value;
            var newUserWithAccess = document.querySelector('input[name="newUserWithAccess"]').value;
            if (currentUsersName === newUserWithAccess) {
                $scope.statusString = "This user already has access!";
            }                
            else {
                var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

                var idToShare = document.querySelector('input[name="DayExpenses.Id"]').value;
                var params = "NewUserWithAccess=" + encodeURIComponent(newUserWithAccess);

                $http.post(`/DayExpenses/Share/` + idToShare, params, {
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',  // Set content type for form data
                        'RequestVerificationToken': token  // Anti-forgery token
                    }
                })
                    .then(function (response) {

                        if (response.data === "Done!") {
                            $scope.statusString = response.data;
                            $('#staticBackdrop').modal('hide');

                            var dayIndex = $scope.days.findIndex(function (day) {
                                return day.dayExpenses.id == idToShare;
                            });

                            $scope.showToast('success', 'Success!', 'Access for this day was successfully shared.');
                            $scope.triggerAnimation(dayIndex % 5, 'edit');
                        }
                        else
                            $scope.statusString = response.data;
                    });
            }  
        };

        // Edit DayExpenses
        $scope.showModalForDayExpensesEdit = function(dayId) {
            $http.get('/DayExpenses/EditDayExpenses/' + dayId).then(function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
                compiledContent = $compile(modalContent)($scope);
                $scope.day = {
                    date: new Date(document.querySelector('input[name="date"]').value),
                    participantList: document.querySelector('input[name="participants"]').value
                };
            });
        };

        $scope.editDayExpenses = function () {           

            var date = ($scope.day && $scope.day.date !== undefined)
                ? $filter('date')($scope.day.date, 'yyyy-MM-ddTHH:mm:ss')
                : "None";
            var participantsList = ($scope.day && $scope.day.participantList !== undefined)
                ? $scope.day.participantList
                : "";

            if ($scope.day.date === undefined || $scope.day.participantList === undefined) {
                if ($scope.day.date === undefined)
                    $scope.dateError = "The value 'None' is not valid for Date";
                else
                    $scope.dateError = undefined;

                if ($scope.day.participantList === undefined)
                    $scope.participantsError = "Add some participants";
                else
                    $scope.participantsError = undefined;

                return;
            }

            var idToEdit = document.querySelector('input[name="DayExpenses.Id"]').value;
            var peopleWithAccessList = JSON.parse(document.querySelector('input[name="DayExpenses.PeopleWithAccess"]').value);
            var token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            var params = "Date=" + encodeURIComponent(date) +
                "&ParticipantsList=" + encodeURIComponent(participantsList) +
                "&PeopleWithAccessList=" + encodeURIComponent(peopleWithAccessList);

            $http.post(`/DayExpenses/Edit/` + idToEdit, params, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',  // Set content type for form data
                    'RequestVerificationToken': token  // Anti-forgery token
                }
            })
                .then(function (response) {
                    // Check if response contains div with class modal-body
                    if (typeof response.data === 'string' && response.data.indexOf("<div class=\"modal-body\">") >= 0) {
                        modalContent = angular.element(document.querySelector('#modal-content'));
                        modalContent.html(response.data);
                        compiledContent = $compile(modalContent)($scope);
                    }
                    else {
                        $scope.day = { date: '', participantList: '' };
                        $('#staticBackdrop').modal('hide');

                        var dayIndex = $scope.days.findIndex(function (day) {
                            return day.dayExpenses.id == idToEdit;
                        });

                        if (dayIndex !== -1) {
                            $scope.days.splice(dayIndex, 1, response.data);
                            $scope.pagedDays[$scope.currentPage].splice(dayIndex % 5, 1, response.data);
                        }

                        $scope.showToast('success', 'Success!', 'Day was successfully edited.');
                        $scope.triggerAnimation(dayIndex % 5, 'edit');   
                    }
                });
        };

        // Delete DayExpenses
        $scope.showModalForDayExpensesDelete = function(dayId) {
            $http.get('/DayExpenses/DeleteDayExpenses/' + dayId).then(function (response) {
                modalContent = angular.element(document.querySelector('#modal-content'));
                modalContent.html(response.data);
                compiledContent = $compile(modalContent)($scope);
            });
        };

        $scope.deleteDayExpenses = function () {
            var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            var idToRemove = document.querySelector('input[name="DayExpenses.Id"]').value;

            $http.post(`/DayExpenses/Delete/` + idToRemove, {}, {
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',  // Set content type for form data
                    'RequestVerificationToken': token  // Anti-forgery token
                }
            })
                .then(function (response) {
                    $('#staticBackdrop').modal('hide');                    

                    var dayIndex = $scope.days.findIndex(function (day) {
                        return day.dayExpenses.id == idToRemove;
                    });

                    if (dayIndex > -1) {
                        $scope.days.splice(dayIndex, 1);
                    }

                    var currentPage = $scope.currentPage;
                    $scope.filterPagedDays();

                    // Move one page back if current not exists now
                    if (currentPage > $scope.pagedDays.length - 1)
                        currentPage -= 1;

                    $scope.currentPage = currentPage;

                    $scope.showToast('success', 'Success!', 'Day was successfully deleted.');                                        
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