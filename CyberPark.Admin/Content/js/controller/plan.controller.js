(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('PlanController', PlanController);
    module.factory('$planEditModal', function () {
        var modal = $('#planEditModal');
        return modal;
    });

    PlanController.$inject = ['$http', 'GlobalAlertService', '$planEditModal'];
    function PlanController($http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */
        var vm = this;

        //data
        vm.plans = [];
        vm.editingPlan = {};

        //filter
        vm.filter = {
            active: true,inactive: true,personal: true,business: true,clothed: true,
            naked: true,term: true,noTerm: true, adsl: true,vdsl: true,ufb: true,
            express: function (value, index, array) {
                //active
                if ((value.isActive && !vm.filter.active) || (!value.isActive && !vm.filter.inactive)) {
                    return false;
                }

                //personal / business
                if ((value.AccountType === 'Personal' && !vm.filter.personal) || (value.accountType === 'Business' && !vm.filter.business)) {
                    return false;
                }

                //Clothed / Naked
                if ((value.isNaked && !vm.filter.naked) || (!value.isNaked && !vm.filter.clothed)) {
                    return false;
                }

                //Term / No-Term
                if ((value.monthsOfTerm == 0 && !vm.filter.noTerm) || (value.monthsOfTerm != 0 && !vm.filter.term)) {
                    return false;
                }

                //ADSL / VDSL / UFB
                if ((value.broadbandType === 'ADSL' && !vm.filter.adsl) ||
                    (value.broadbandType === 'VDSL' && !vm.filter.vdsl) ||
                    (value.broadbandType === 'UFB' && !vm.filter.ufb)) {
                    return false;
                }
                return true;
            }
        };

        //load plans
        vm.loadPlans = function () {
            vm.loading = true;
            $http.get('/api/plan').success(function (result) {
                vm.loading = false;
                if (result.success) {
                    //show plans
                    vm.plans = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load plans");
                }
            }).error(function () {
                vm.loading = false;
                gblAlrtSrv.error("Failed to load plans");
            });
        }

        //new
        vm.createPlan = function () {
            vm.editingPlan = {
                name: 'new plan',
                monthlyPrice: 0.00,
                monthsOfTerm: 0,
                accountType: 'Personal',
                broadbandType: 'ADSL',
                pstnCount: 0,
                voipCount: 0,
                displayPriority:0
            };
            $editModal.modal('show');
        }

        //edit
        vm.editPlan = function (plan) {
            vm.editingPlan = {};
            angular.extend(vm.editingPlan, plan);
            $editModal.modal('show');
        }

        //update
        vm.savePlan = function () {
            vm.editingPlan.saving = true;
            if (vm.editingPlan.id) {
                //update
                $http.put('/api/plan', vm.editingPlan).success(function (result) {
                    vm.editingPlan.saving = false;
                    vm.editingPlan = {};
                    $editModal.modal('hide');
                    vm.loadPlans();
                    if (result.success) {
                        gblAlrtSrv.success("Succeed to save plan");
                    } else {
                        gblAlrtSrv.error("Failed to save plan");
                    }
                }).error(function () {
                    vm.editingPlan.saving = false;
                    vm.editingPlan = {};
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to save plan");
                });
            } else {
                //save
                $http.post('/api/plan', vm.editingPlan).success(function (result) {
                    vm.editingPlan.saving = false;
                    vm.editingPlan = null;
                    $editModal.modal('hide');
                    vm.loadPlans();
                    if (result.success) {
                        vm.loadPlans();
                        gblAlrtSrv.success("Succeed to create plan");
                    } else {
                        gblAlrtSrv.error("Failed to create plan");
                    }
                }).error(function () {
                    vm.editingPlan.saving = false;
                    vm.editingPlan = null;
                    $editModal.modal('hide');
                    gblAlrtSrv.error("Failed to create plan");
                });
            }
        }

        //query remove
        vm.queryRemove = function (plan) {
            plan.confirmRemove = true;
        }

        //cancel remove
        vm.cancelRemove = function (plan) {
            plan.confirmRemove = false;
        }

        //remove
        vm.removePlan = function (plan) {
            plan.confirmRemove = false;
            plan.removing = true;
            $http.delete('/api/plan/' + plan.id).success(function (result) {
                plan.removing = false;
                if (result.success && result.data) {
                    //remove plan
                    vm.loadPlans();
                    gblAlrtSrv.success("Succeed to remove plan: " + plan.name);
                } else {
                    gblAlrtSrv.error("Failed to remove plan: " + plan.name);
                }
            }).error(function () {
                plan.removing = false;
                gblAlrtSrv.error("Failed to remove plan " + plan.name);
            });
        }

        //set activity
        vm.setPlanActivity = function (plan) {
            plan.settingActivity = true;
            plan.isActive = !plan.isActive;
            $http.put('/api/plan', plan).success(function (result) {
                plan.settingActivity = false;
                if (result.success) {
                    //show plans
                    plan = result.data;
                    gblAlrtSrv.success("Succeed to set activity of plan: " + plan.name);
                } else {
                    gblAlrtSrv.error("Failed to set activity of plan: " + plan.name);
                }
            }).error(function () {
                lan.settingActivity = false;
                gblAlrtSrv.error("Failed to set activity of plan: " + plan.name);
            });
        }

        vm.loadPlans();
    }
})();
