(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('InvoiceController', InvoiceController);

    InvoiceController.$inject = ['$scope', '$http', 'GlobalAlertService'];
    function InvoiceController($scope, $http, gblAlrtSrv) {
        /* jshint validthis:true */
        var vm = this;
        vm.accoutId = null;
        vm.invoices = null;

        //subscribe
        $scope.$on('account.loaded', function (event, accountId) {
            vm.load(accountId);
        });

        /* load invoices */
        vm.load = function (accountId) {
            //load invoice
            vm.accoutId = accountId;
            $http.get('/api/account/' + accountId + '/invoice/recent', { params: { rows: 10 } }).success(function (result) {
                if (result.success) {
                    //show invoice
                    vm.invoices = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load invoices:" + result.message);
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load invoices");
            });
        }

        /*void invoice */
        vm.void = function (invId) {
            $http.delete('/api/account/invoice/' + invId).success(function (result) {
                if (result.success) {
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accoutId);
                    gblAlrtSrv.success("Succeed to void invoice");
                } else {
                    gblAlrtSrv.error("Failed to void invoice:" + result.message);
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to void invoice");
            });
        }
        /*issue invoice */
        vm.reissue = function (invId) {
            $http.get('/api/account/' + vm.accoutId + '/invoice/issue', { params: {invoiceId:invId } }).success(function (result) {
                if (result.success) {
                    //told account to refresh
                    $scope.$emit("account.refresh", vm.accoutId);
                    gblAlrtSrv.success("Succeed to issue invoice");
                } else {
                    gblAlrtSrv.error("Failed to issue invoice:" + result.message);
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to issue invoice");
            });
        }

        /*search*/
        vm.search = function () {
            $http.get('/api/invoice', { params: vm.searcher }).success(function (result) {
                if (result.success) {
                    //show invoice
                    vm.invoices = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load invoice");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load invoice");
            });
        }
    }
})();