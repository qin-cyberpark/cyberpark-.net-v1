(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('ApplicationController', ApplicationController);

    ApplicationController.$inject = ['$scope', '$http'];
    function ApplicationController($scope, $http) {
        var vm = this;
        vm.application = {
            payMonthly: "true",
            amount:function(){
                if (this.payMonthly === 'false') {
                    return this.plan.MonthlyPrice * 12;
                } else {
                    return this.plan.MonthlyPrice;
                }
            }
        }
        
        vm.init = function (address, planJson, selectedPlanId) {
            vm.plans = planJson;
            vm.application.address = address;
            angular.forEach(vm.plans, function (plan) {
                if (plan.Id === selectedPlanId) {
                    vm.application.plan = plan;
                    vm.application.payMonthly = plan.IsBusiness? 'true' : 'false';
                }
            });
            //console.log(vm.plans);
        }

        vm.select = function (plan) {
            vm.application.plan = plan;
        }
    }
})();