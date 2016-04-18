(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('WarningController', WarningController);

    WarningController.$inject = ['$scope', '$http', 'GlobalAlertService'];
    function WarningController($scope, $http, gblAlrtSrv) {
        /* jshint validthis:true */
        var vm = this;
        vm.warnings = null;

        /* load warning */
        vm.load = function () {
            //load account
            $http.get('/api/sys/warning').success(function (result) {
                if (result.success) {
                    //show account
                    vm.warnings = result.data;
                } else {
                    console.log(result);
                    gblAlrtSrv.error("Failed to load warnings");
                }
            }).error(function (data) {
                gblAlrtSrv.error("Failed to load warnings");
            });
        }

        vm.clearWarning = function (warning) {
            //load account
            $http.delete('/api/sys/warning/' + warning.id).success(function (result) {
                if (result.success) {
                    //show account
                    vm.warnings.splice(vm.warnings.indexOf(warning), 1);
                } else {
                    gblAlrtSrv.error("Failed to clear warnings");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to clear warnings");
            });
        }

        vm.load();
    }
})();
