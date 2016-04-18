(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('ServicePriceController', ServicePriceController);
    module.factory('$priceEditModal', function () {
        var modal = $('#priceEditModal');
        return modal;
    });

    ServicePriceController.$inject = ['$http', 'GlobalAlertService', '$priceEditModal'];
    function ServicePriceController($http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */
        var vm = this;

        //data
        vm.services = [];
        vm.editingService = null;

        //filter
        vm.filter = {
            active: true, inactive: true, isPersonal: true, isBusiness: true,isMonthly:true,isOneOff:true
            ,express: function (value, index, array) {
                //active
                if ((value.isActive && !vm.filter.active) || (!value.isActive && !vm.filter.inactive)) {
                    return false;
                }

                //personal / business
                if ((value.isPersonal && !vm.filter.isBusiness) || (value.isBusiness && !vm.filter.isPersonal)) {
                    return false;
                }

                //Monthly / one-off
                if ((value.isMonthly && !vm.filter.isOneOff) || (!value.isMonthly && !vm.filter.isOneOff)) {
                    return false;
                }

                ////ADSL / VDSL / UFB
                //if ((value.broadbandType === 'ADSL' && !vm.filter.adsl) ||
                //    (value.broadbandType === 'VDSL' && !vm.filter.vdsl) ||
                //    (value.broadbandType === 'UFB' && !vm.filter.ufb)) {
                //    return false;
                //}
                return true;
            }
        };

        //load services
        vm.loadServices = function () {
            vm.loading = true;
            $http.get('/api/servicePrice').success(function (result) {
                vm.loading = false;
                if (result.success) {
                    //show services
                    vm.services = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load services");
                }
            }).error(function () {
                vm.loading = false;
                gblAlrtSrv.error("Failed to load services");
            });
        }

        //new
        vm.createService = function () {
            vm.editingService = {
                name: 'New Service',
                price: 0.00,
                serviceType: 'Broadband',
                serviceSubType: 'ADSL',
                isBusiness:false,
                isMonthly:true
            };
            $editModal.modal('show');
        }

        //edit
        vm.editService = function (service) {
            vm.editingService = {};
            angular.extend(vm.editingService, service);
            $editModal.modal('show');
        }

        //update
        vm.saveService = function () {
            vm.editingService.saving = true;
            if (vm.editingService.id) {
                //update
                $http.put('/api/servicePrice', vm.editingService).success(function (result) {
                    vm.editingService.saving = false;
                    vm.editingService = null;

                    $editModal.modal('hide');
                    if (result.success) {
                        vm.loadServices();
                        gblAlrtSrv.success("Succeed to save service");
                    } else {
                        gblAlrtSrv.error("Failed to save service");
                    }
                }).error(function () {
                    vm.editingService.saving = false;
                    vm.editingService = null;
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to save service");
                });
            } else {
                //save
                $http.post('/api/servicePrice', vm.editingService).success(function (result) {
                    vm.editingService.saving = false;
                    vm.editingService = null;
                    $editModal.modal('hide');
                    if (result.success) {
                        //remove ori service
                        vm.loadServices();
                        gblAlrtSrv.success("Succeed to create service");
                    } else {
                        gblAlrtSrv.error("Failed to create service");
                    }
                }).error(function () {
                    vm.editingService.saving = false;
                    vm.editingService = null;
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to create service");
                });
            }
        }

        //query remove
        vm.queryRemove = function (service) {
            service.confirmRemove = true;
        }

        //cancel remove
        vm.cancelRemove = function (service) {
            service.confirmRemove = false;
        }

        //remove
        vm.removeService = function (service) {
            service.confirmRemove = false;
            service.removing = true;
            $http.delete('/api/servicePrice/' + service.id).success(function (result) {
                service.removing = false;
                if (result.success && result.data) {
                    //remove service
                    vm.services.splice(vm.services.indexOf(service), 1);
                    gblAlrtSrv.success("Succeed to remove service: " + service.name);
                } else {
                    gblAlrtSrv.error("Failed to remove service: " + service.name);
                }
            }).error(function () {
                service.removing = false;
                gblAlrtSrv.error("Failed to remove service " + service.name);
            });
        }

        //set activity
        vm.setServiceActivity = function (service) {
            service.settingActivity = true;
            service.isActive = !service.isActive;
            $http.put('/api/servicePrice', service).success(function (result) {
                service.settingActivity = false;
                if (result.success) {
                    service = result.data;
                    gblAlrtSrv.success("Succeed to set activity of service: " + service.name);
                } else {
                    gblAlrtSrv.error("Failed to set activity of service: " + service.name);
                }
            }).error(function () {
                service.settingActivity = false;
                gblAlrtSrv.error("Failed to set activity of service: " + service.name);
            });
        }

        vm.loadServices();
    }
})();
