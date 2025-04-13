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

expensesCalculatorApp.service('rowAnimationService', ['$timeout', function ($timeout) {
	var animatedRowIndex = null;
	var animationType = null;

	return {
		triggerAnimation: function (index, type) {
			animatedRowIndex = index;
			animationType = type;

			$timeout(function () {
				animatedRowIndex = null;
				animationType = null;
			}, 500);
		},
		getAnimatedRowIndex: function () {
			return animatedRowIndex;
		},
		getAnimationType: function () {
			return animationType;
		}
	};
}]);

expensesCalculatorApp.service('shareDayExpensesService', ['$http', '$compile', '$q', function ($http, $compile, $q) {

	// Method to fetch and display the modal content for sharing day expenses
	this.showModalForDayExpensesShare = function (dayId, $scope) {
		var deferred = $q.defer();  // Use $q to handle asynchronous behavior

		$http.get('/DayExpenses/ShareDayExpenses/' + dayId).then(function (response) {
			var modalContent = angular.element(document.querySelector('#modal-content'));
			modalContent.html(response.data);

			// Compile the new content and link it to the scope
			var compiledContent = $compile(modalContent)($scope);
			deferred.resolve(response.data);
		});

		return deferred.promise;  // Return promise for chaining
	};

	// Method to handle sharing of day expenses
	this.shareDayExpenses = function (currentUsersName, newUserWithAccess, dayId, token) {
		var deferred = $q.defer();

		// Check if the user already has access
		if (currentUsersName === newUserWithAccess) {
			deferred.reject("This user already has access!");
		} else {
			var params = "NewUserWithAccess=" + encodeURIComponent(newUserWithAccess);

			// Make the POST request to share day expenses
			$http.post('/DayExpenses/Share/' + dayId, params, {
				headers: {
					'Content-Type': 'application/x-www-form-urlencoded',
					'RequestVerificationToken': token
				}
			}).then(function (response) {
				if (response.data === "Done!") {
					deferred.resolve(response.data);
				} else {
					deferred.reject(response.data);
				}
			});
		}

		return deferred.promise;  // Return promise for chaining
	};

}]);

expensesCalculatorApp.service('editDayExpensesService', ['$http', '$compile', '$filter', function ($http, $compile, $filter) {

	this.showEditModal = function (dayId, $scope) {
		return $http.get('/DayExpenses/EditDayExpenses/' + dayId).then(function (response) {
			const modalContent = angular.element(document.querySelector('#modal-content'));
			modalContent.html(response.data);
			$compile(modalContent)($scope);

			$scope.day = {
				date: new Date(document.querySelector('input[name="date"]').value),
				participantList: document.querySelector('input[name="participants"]').value
			};
		});
	};

	this.editDay = function ($scope) {
		const idToEdit = document.querySelector('input[name="DayExpenses.Id"]').value;
		const peopleWithAccessList = JSON.parse(document.querySelector('input[name="DayExpenses.PeopleWithAccess"]').value);
		const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

		const date = ($scope.day && $scope.day.date !== undefined)
			? $filter('date')($scope.day.date, 'yyyy-MM-ddTHH:mm:ss')
			: "None";
		const participantsList = ($scope.day && $scope.day.participantList !== undefined)
			? $scope.day.participantList
			: "";

		// Validate input
		if ($scope.day.date === undefined || $scope.day.participantList === undefined) {
			$scope.dateError = $scope.day.date === undefined ? "The value 'None' is not valid for Date" : undefined;
			$scope.participantsError = $scope.day.participantList === undefined ? "Add some participants" : undefined;
			return;
		}

		const params = "Date=" + encodeURIComponent(date) +
			"&ParticipantsList=" + encodeURIComponent(participantsList) +
			"&PeopleWithAccessList=" + encodeURIComponent(JSON.stringify(peopleWithAccessList));

		return $http.post(`/DayExpenses/Edit/` + idToEdit, params, {
			headers: {
				'Content-Type': 'application/x-www-form-urlencoded',
				'RequestVerificationToken': token
			}
		}).then(function (response) {
			if (typeof response.data === 'string' && response.data.indexOf("<div class=\"modal-body\">") >= 0) {
				const modalContent = angular.element(document.querySelector('#modal-content'));
				modalContent.html(response.data);
				$compile(modalContent)($scope);
			} else {
				// Modal closing and data updating
				$scope.day = { date: '', participantList: '' };
				const bsModal = bootstrap.Modal.getInstance(document.getElementById('staticBackdrop'));
				bsModal && bsModal.hide();

				const dayIndex = $scope.days ? $scope.days.findIndex(day => day.dayExpenses.id == idToEdit) : 0;

				if ($scope.days && dayIndex !== -1) {
					$scope.days.splice(dayIndex, 1, response.data);
					$scope.pagedDays[$scope.currentPage].splice(dayIndex % 5, 1, response.data);
					$scope.triggerAnimation(dayIndex % 5, 'edit');
				}

				else if ($scope.dayExpenses) {
					$scope.dayExpenses = response.data;
					$scope.triggerAnimation(undefined, 'edit');
				}

				$scope.showToast('success', 'Success!', 'Day was successfully edited.');
				
			}
		});
	};
}]);

expensesCalculatorApp.service('deleteDayExpensesService', ['$http', '$compile', '$window', function ($http, $compile, $window) {

	this.showDeleteModal = function (dayId, $scope) {
		$http.get('/DayExpenses/DeleteDayExpenses/' + dayId)
			.then(function (response) {
				var modalContent = angular.element(document.querySelector('#modal-content'));
				modalContent.html(response.data);
				$compile(modalContent)($scope);
			});
	};

	this.deleteDay = function ($scope) {
		var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
		var idToRemove = document.querySelector('input[name="DayExpenses.Id"]').value;

		$http.post(`/DayExpenses/Delete/` + idToRemove, {}, {
			headers: {
				'Content-Type': 'application/x-www-form-urlencoded',
				'RequestVerificationToken': token
			}
		})
			.then(function (response) {
				$('#staticBackdrop').modal('hide');

				var dayIndex = $scope.days ? $scope.days.findIndex(function (day) {
					return day.dayExpenses.id == idToRemove;
				}) : 0;

				if ($scope.days && dayIndex > -1) {
					$scope.days.splice(dayIndex, 1);

					var currentPage = $scope.currentPage;
					$scope.filterPagedDays();

					if (currentPage > $scope.pagedDays.length - 1) {
						currentPage -= 1;
					}

					$scope.currentPage = currentPage;
				}

				else if ($scope.dayExpenses) {
					$window.location.href = '/DayExpenses';
				}

				$scope.showToast('success', 'Success!', 'Day was successfully deleted.');			
			});
	};

}]);

expensesCalculatorApp.controller('DayExpensesCtrl', ['$scope', '$http', '$filter', '$compile',
	'toastService', 'rowAnimationService', 'shareDayExpensesService', 'editDayExpensesService', 'deleteDayExpensesService',
	function ($scope, $http, $filter, $compile,
		toastService, rowAnimationService, shareDayExpensesService, editDayExpensesService, deleteDayExpensesService) {

		// Expose the toastService's toasts array to the scope (if you need to access it in the view)
		$scope.toasts = toastService.toasts;

		// Use the showToast method from the toastService
		$scope.showToast = function (type, title, message) {
			toastService.showToast(type, title, message);
		};

		// Animations
		$scope.triggerAnimation = function (index, type) {
			rowAnimationService.triggerAnimation(index, type);
		};

		$scope.getAnimatedRowIndex = function () {
			return rowAnimationService.getAnimatedRowIndex();
		};

		$scope.getAnimationType = function () {
			return rowAnimationService.getAnimationType();
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

		// Share DayExpenses
		$scope.showModalForDayExpensesShare = function (dayId) {
			shareDayExpensesService.showModalForDayExpensesShare(dayId, $scope).then(function (data) {
				// Success - you can handle any post-modal logic here
			}, function (error) {
				console.error(error);
			});
		};

		$scope.shareDayExpenses = function () {
			var currentUsersName = document.querySelector('input[name="currentUsersName"]').value;
			var newUserWithAccess = document.querySelector('input[name="newUserWithAccess"]').value;
			var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
			var idToShare = document.querySelector('input[name="DayExpenses.Id"]').value;

			shareDayExpensesService.shareDayExpenses(currentUsersName, newUserWithAccess, idToShare, token).then(function (response) {
				// Success: Update status message, close modal, etc.
				$scope.statusString = response;
				$('#staticBackdrop').modal('hide');

				var dayIndex = $scope.days.findIndex(function (day) {
					return day.dayExpenses.id == idToShare;
				});

				$scope.showToast('success', 'Success!', 'Access for this day was successfully shared.');
				$scope.triggerAnimation(dayIndex % 5, 'edit');
			}, function (error) {
				$scope.statusString = error;
			});
		};

		// Edit DayExpenses
		$scope.showModalForDayExpensesEdit = function (dayId) {
			editDayExpensesService.showEditModal(dayId, $scope);
		};

		$scope.editDayExpenses = function () {
			editDayExpensesService.editDay($scope);
		};

		// Delete DayExpenses        
		$scope.showModalForDayExpensesDelete = function (dayId) {
			deleteDayExpensesService.showDeleteModal(dayId, $scope);
		};

		$scope.deleteDayExpenses = function () {
			deleteDayExpensesService.deleteDay($scope);
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

expensesCalculatorApp.controller('DayExpensesChecksCtrl', ['$scope', '$http', '$filter',
	'toastService', 'rowAnimationService', 'shareDayExpensesService', 'editDayExpensesService', 'deleteDayExpensesService',
	function ($scope, $http, $filter,
		toastService, rowAnimationService, shareDayExpensesService, editDayExpensesService, deleteDayExpensesService) {
	var dayExpensesId = angular.element(document.querySelector('#dayExpensesId')).val();

	$http.get('/DayExpenses/GetDayById/' + dayExpensesId)
		.then(getChecksSuccessfulCallback, getChecksErrorCallback);

	function getChecksSuccessfulCallback(response) {
		$scope.dayExpenses = response.data;
		$scope.checks = response.data.dayExpenses.checks;
		$scope.filterPagedChecks();        
	}
	function getChecksErrorCallback(error) {
		console.log(error);
	}

	// Expose the toastService's toasts array to the scope (if you need to access it in the view)
	$scope.toasts = toastService.toasts;
	
	// Use the showToast method from the toastService
	$scope.showToast = function (type, title, message) {
		toastService.showToast(type, title, message);
	};
	
	// Animations
	$scope.triggerAnimation = function (index, type) {
		rowAnimationService.triggerAnimation(index, type);
	};
	
	$scope.getAnimatedRowIndex = function () {
		return rowAnimationService.getAnimatedRowIndex();
	};
	
	$scope.getAnimationType = function () {
		return rowAnimationService.getAnimationType();
	};

	// Share DayExpenses
	$scope.showModalForDayExpensesShare = function (dayId) {
		shareDayExpensesService.showModalForDayExpensesShare(dayId, $scope).then(function (data) {
			// Success - you can handle any post-modal logic here
		}, function (error) {
			console.error(error);
		});
	};
	
	$scope.shareDayExpenses = function () {
		var currentUsersName = document.querySelector('input[name="currentUsersName"]').value;
		var newUserWithAccess = document.querySelector('input[name="newUserWithAccess"]').value;
		var token = document.querySelector('input[name="__RequestVerificationToken"]').value;
		var idToShare = document.querySelector('input[name="DayExpenses.Id"]').value;
	
		shareDayExpensesService.shareDayExpenses(currentUsersName, newUserWithAccess, idToShare, token).then(function (response) {
			// Success: Update status message, close modal, etc.
			$scope.statusString = response;
			$('#staticBackdrop').modal('hide');
	
			$scope.showToast('success', 'Success!', 'Access for this day was successfully shared.');
			$scope.triggerAnimation(undefined, 'edit');
		}, function (error) {
			$scope.statusString = error;
		});
	};

	// Edit DayExpenses
	$scope.showModalForDayExpensesEdit = function (dayId) {
		editDayExpensesService.showEditModal(dayId, $scope);
	};
	
	$scope.editDayExpenses = function () {
		editDayExpensesService.editDay($scope);
	};

	// Delete DayExpenses        
	$scope.showModalForDayExpensesDelete = function (dayId) {
		deleteDayExpensesService.showDeleteModal(dayId, $scope);
	};
	
	$scope.deleteDayExpenses = function () {
		deleteDayExpensesService.deleteDay($scope);
	};

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