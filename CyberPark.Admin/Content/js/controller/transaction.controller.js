(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('TransactionController', TransactionController);
    module.factory('$transEditModal', function () {
        var modal = $('#transEditModal');
        return modal;
    });

    TransactionController.$inject = ['$scope', '$http', 'GlobalAlertService', '$transEditModal'];
    function TransactionController($scope, $http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */

        var vm = this;
        vm.transactions = null;
        vm.accountId = null;
        vm.editingTransaction = {};
        vm.alerts = [];

        /*alert*/
        vm.success = function (message) {
            gblAlrtSrv.success(message, vm.alerts);
        }
        vm.error = function (message) {
            gblAlrtSrv.error(message, vm.alerts);
        }
        vm.closeAlert = function (alert) {
            gblAlrtSrv.closeAlert(alert, vm.alerts);
        }

        //
        vm.formatDate = function (dt) {
            return dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate() + " "
                + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
        }

        //subscribe
        $scope.$on('account.loaded', function (event, accountId) {
            vm.load(accountId);
        });

        /* load transactions */
        vm.load = function (accountId) {
            vm.accountId = accountId;
            //load adjustment
            $http.get('/api/account/' + accountId + '/transaction/recent', { params: { rows: 10 } }).success(function (result) {
                if (result.success) {
                    //show transactions
                    vm.transactions = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load transactions");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load transactions");
            });
        }

        /* open edit transaction modal */
        vm.create = function () {
            vm.editingTransaction = {
                accountId: vm.accountId
            };
            vm.alerts.length = 0;
            $editModal.modal('show');
        }

        /* save service */
        vm.save = function () {
            //new save
            $http.post('/api/transaction/', vm.editingTransaction).success(function (result) {
                if (result.success) {
                    vm.load(vm.accountId);
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accountId);
                    vm.success("transaction added");
                } else {
                    vm.error("Failed to add transaction");
                }
                $editModal.modal('hide');
            }).error(function () {
                vm.error("Failed to add transaction");
                $editModal.modal('hide');
            });
        }

        /* remove transaction */
        vm.remove = function (trans) {
            $http.delete('/api/transaction/' + trans.id).success(function (result) {
                if (result.success) {
                    vm.load(vm.accountId);
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accountId);
                    gblAlrtSrv.success("transaction deleted");
                } else {
                    gblAlrtSrv.error("Failed to delete transaction");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to delete transaction");
            });
        }
    }
})();