(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('AccountController', AccountController);

    AccountController.$inject = ['$scope', '$http', 'GlobalAlertService'];
    function AccountController($scope, $http, gblAlrtSrv) {
        /* jshint validthis:true */

        //subscribe
        $scope.$on('account.refresh', function (event, accountId) {
            vm.load(accountId);
        });

        var vm = this;
        vm.accounts = [];
        vm.account = {};

        /* general info */
        vm.acctGeneral = {};

        //searcher
        vm.searcher = {
            customerId: "", accountId: "", name: "", address: "",
            asid: "", pstn: "", voip: "", status:""
        };

        /* account List */
        vm.init = function (condition) {
            vm.searcher = condition;
            vm.search();
        }

        vm.search = function () {
            $http.get('/api/account', { params: vm.searcher }).success(function (result) {
                if (result.success) {
                    //show accounts
                    vm.accounts = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load accounts");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load accounts");
            });
        }

        /* load account */
        vm.load = function (acctId) {
            //load account
            $http.get('/api/account/' + acctId).success(function (result) {
                if (result.success) {
                    //show account
                    vm.account = result.data;
                    vm.acctGeneral = {};
                    angular.extend(vm.acctGeneral,vm.account);
                    $scope.$broadcast("account.loaded", vm.account.id);
                } else {
                    gblAlrtSrv.error("Failed to load account");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load account");
            });
        }

        /* copy contact from customer */
        vm.copyContactFromCustomer = function (customer) {
            vm.acctGeneral.title = customer.title,
            vm.acctGeneral.firstName = customer.firstName,
            vm.acctGeneral.lastName = customer.lastName,
            vm.acctGeneral.mobile = customer.mobile,
            vm.acctGeneral.email = customer.email
        }

        /* copy identity from customer */
        vm.copyIdentityFromCustomer = function (customer) {
            vm.acctGeneral.identityType = customer.identityType,
            vm.acctGeneral.identityNumber = customer.identityNumber
        }

        /* update general info */
        vm.updateAccountGeneral = function () {
            //update general
            $http.put('/api/account/', vm.acctGeneral).success(function (result) {
                if (result.success) {
                    vm.account = result.data;
                    vm.acctGeneral = {};
                    angular.extend(vm.acctGeneral, vm.account);
                    $scope.$emit("customer.refresh", vm.account.customerId);
                    gblAlrtSrv.success("Account general infomation updated");
                } else {
                    gblAlrtSrv.error("Failed to updated account general infomation");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to updated account general infomation");
            });
        }
    }
})();
