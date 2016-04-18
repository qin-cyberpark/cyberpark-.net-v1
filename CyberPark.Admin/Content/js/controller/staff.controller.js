(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('StaffController', StaffController);
    module.factory('$staffEditModal', function () {
        var modal = $('#staffEditModal');
        return modal;
    });

    StaffController.$inject = ['$http', 'GlobalAlertService', '$staffEditModal'];
    function StaffController($http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */
        var vm = this;

        //data
        vm.staffs = [];
        vm.editingStaff = {};

        //load staff
        vm.loadStaffs = function () {
            $http.get('/api/staff').success(function (result) {
                vm.loading = false;
                if (result.success) {
                    //show plans
                    vm.staffs = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load staffs");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load staffs");
            });
        }

        //new
        vm.createStaff = function () {
            vm.editingStaff = {
                name: 'name',
            };
            $editModal.modal('show');
        }

        //edit
        vm.editStaff = function (model) {
            vm.editingStaff = {};
            angular.extend(vm.editingStaff, model);
            $editModal.modal('show');
        }

        //update
        vm.saveStaff = function () {
            vm.editingStaff.saving = true;
            if (vm.editingStaff.password !=null && vm.editingStaff.confirmPassword !== vm.editingStaff.password) {
                gblAlrtSrv.error("twice password are different");
                vm.editingStaff.saving = false;
                return;
            }
            if (vm.editingStaff.id) {
                //update
                $http.put('/api/staff', vm.editingStaff).success(function (result) {
                    vm.editingStaff = {};
                    $editModal.modal('hide');
                    vm.loadStaffs();
                    if (result.success) {
                        gblAlrtSrv.success("Succeed to save staff");
                    } else {
                        gblAlrtSrv.error("Failed to save staff");
                    }
                }).error(function () {
                    vm.editingStaff = {};
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to save staff");
                }).finally(function(){
                    vm.editingStaff.saving = false;
                });
            } else {
                //save
                $http.post('/api/staff', vm.editingStaff).success(function (result) {
                    vm.editingStaff = null;
                    $editModal.modal('hide');
                    vm.loadPlans();
                    if (result.success) {
                        vm.loadStaffs();
                        gblAlrtSrv.success("Succeed to create staff");
                    } else {
                        gblAlrtSrv.error("Failed to create staff");
                    }
                }).error(function () {
                    vm.editingStaff = null;
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to create staff");
                }).finally(function () {
                    vm.editingStaff.saving = false;
                });
            }
        }

        //remove
        vm.removePlan = function (staff) {
            staff.removing = true;
            $http.delete('/api/plan/' + plan.id).success(function (result) {
                if (result.success && result.data) {
                    //remove plan
                    vm.loadPlans();
                    gblAlrtSrv.success("Succeed to remove staff: " + staff.name);
                } else {
                    gblAlrtSrv.error("Failed to remove staff: " + staff.name);
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to remove staff " + staff.name);
            }).finally(function () {
                staff.removing = false;
            });;
        }
    }
})();
