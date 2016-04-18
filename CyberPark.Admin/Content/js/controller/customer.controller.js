(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('CustomerController', CustomerController);

    CustomerController.$inject = ['$scope', '$http', 'GlobalAlertService'];

    
    function CustomerController($scope, $http, gblAlrtSrv) {
        /* jshint validthis:true */

        var vm = this;

        //list using
        vm.customers = [];

        //view using
        vm.customer = {}
        vm.editingCustomer = {};
        //searcher
        vm.searcher = {
            customerId: "", accountId: "", name: "", address:"",
            asid:"", pstn:"", voip:""
        };

        //subscribe
        $scope.$on('customer.refresh', function (event, customerId) {
            vm.load(customerId);
        });
        
        /* Customer List */
        //load customers
        vm.search = function () {
            $http.get('/api/customer', { params: vm.searcher }).success(function (result) {
                if (result.success) {
                    //show customers
                    vm.customers = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load customers");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load customers");
            });
        }

        /* Customer View */
        //load customer
        vm.load = function (id) {
            $http.get('/api/customer/' + id).success(function (result) {
                if (result.success) {
                    //show customer
                    vm.customer = result.data;
                    vm.editingCustomer = {};
                    angular.extend(vm.editingCustomer,vm.customer);
                } else {
                    gblAlrtSrv.error("Failed to load customer");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load customer");
            });
        }

        
        //update general info
        vm.updateCustomerGeneral = function () {
            $http.put('/api/customer/', vm.editingCustomer).success(function (result) {
                if (result.success) {
                    //show customer
                    vm.customer = result.data;
                    vm.editingCustomer = {};
                    angular.extend(vm.editingCustomer, vm.customer);
                    gblAlrtSrv.success("Customer saved");
                } else {
                    gblAlrtSrv.error("Failed to update customer");
                }
            }).error(function () {
                vm.generalUpdating = false;
                gblAlrtSrv.error("Failed to update customer");
            });
        }
    }
})();
