(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('AvailableBroadbandController', AvailableBroadbandController);
    module.factory('$address', function () {
        return $('#address');
    });

    AvailableBroadbandController.$inject = ['$http', '$address'];
    function AvailableBroadbandController($http, $address) {
        var vm = this;
        vm.result = null;
        vm.address = null;

        vm.onAddressKeypress = function (event) {
            if (event.keyCode === 13) {
                vm.check();
            }
        }

        vm.check = function () {
            vm.plans = null;
            vm.address = $address.val();
            var url = "/api/available-plan?address=" + vm.address + "&isBusiness=" + vm.isBusiness;
            if (vm.address != '') {
                vm.checking = true;
                $http.get(url).success(function (result) {
                    if (result.success) {
                        vm.address = result.data.address;
                        vm.result = result.data;
                        vm.changePlans(vm.result.adsLs);
                    }
                    //l.stop();
                }).finally(function () {
                    vm.checking = false;
                });
            } else {
                alert('Please enter a real address.');
            }
        }

        vm.init = function (address, isBusiness) {
            vm.isBusiness = isBusiness;
            if (address.length > 0) {
                $address.val(address);
                vm.check();
            }
        }

        vm.changePlans= function(plans){
            vm.plans = plans;
        }
    }
})();