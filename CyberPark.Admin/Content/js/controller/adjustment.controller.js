(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('AdjustmentController', AdjustmentController);
    module.factory('$adjEditModal', function () {
        var modal = $('#adjEditModal');
        return modal;
    });

    AdjustmentController.$inject = ['$scope', '$http', 'GlobalAlertService', '$adjEditModal'];
    function AdjustmentController($scope, $http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */

        var vm = this;
        vm.adjustments = null;
        vm.editingAdjustment = {};
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


        //subscribe
        $scope.$on('account.loaded', function (event, accountId) {
            vm.load(accountId);
        });

        /* load adjustments */
        vm.load = function (accountId) {
            //load adjustment
            vm.accountId = accountId;
            $http.get('/api/account/' + accountId + '/adjustment/recent', { params: { rows: 10 } }).success(function (result) {
                if (result.success) {
                    //show adjustment
                    vm.adjustments = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load adjustments");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load adjustments");
            });
        }

        /* open create adjustment modal */
        vm.create = function () {
            vm.editingAdjustment = {
                accountId: vm.accountId
            };
            vm.alerts.length = 0;
            $editModal.modal('show');
        }


        /* save adjustment */
        vm.save = function () {
            //new save
            $http.post('/api/adjustment/', vm.editingAdjustment).success(function (result) {
                if (result.success) {
                    vm.load(vm.accountId);
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accountId);
                    vm.success("adjustment added");
                } else {
                    vm.error("Failed to add adjustment");
                }
                $editModal.modal('hide');
            }).error(function () {
                vm.error("Failed to add adjustment");
                $editModal.modal('hide');
            });
        }

        /* remove adjustment */
        vm.remove = function (adj) {
            $http.delete('/api/adjustment/' + adj.id).success(function (result) {
                if (result.success) {
                    vm.load(vm.accountId);
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accountId);
                    gblAlrtSrv.success("adjustment deleted");
                } else {
                    gblAlrtSrv.error("Failed to delete adjustment");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to delete adjustment");
            });
        }
    }
})();