(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('ExternalBillController', ExternalBillController);

    ExternalBillController.$inject = ['$scope', '$http', 'GlobalAlertService'];
    function ExternalBillController($scope, $http, gblAlrtSrv) {
        /* jshint validthis:true */


        var vm = this;

        //load bill
        vm.load = function (id) {
            vm.fileId = id;
            $http.get('/api/externalbill/' + id).success(function (result) {
                if (result.success) {
                    //show unmatched
                    vm.data = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load external bill");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load external bill");
            });
        }

        /*build params*/
        vm.buildParams = function (number, isCall) {
            return {
                BillId: vm.fileId,
                Number: number, 
                isCall: isCall
            }
        }

        vm.remove = function (group, isCall) {
            var arr;
            if (isCall) {
                arr = vm.data.unmatchedCalls;               
            } else {
                arr = vm.data.unmatchedServices;
            }
            arr.splice(arr.indexOf(group), 1);
        }

        /* ignore service */
        vm.ignore = function (group, isCall) {
            //ignore service
            var params = vm.buildParams(group.number, isCall);

            $http.put('/api/externalbill/ignore', params).success(function (result) {
                if (result.success) {
                    gblAlrtSrv.success("Number " + group.number + " is ignored");
                    vm.remove(group, isCall);
                } else {
                    gblAlrtSrv.error("Failed to ignore");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to ignored");
            });
        }

        /* match service */
        vm.match = function (group, isCall) {
            //match service
            var params = vm.buildParams(group.number, isCall);

            $http.put('/api/externalbill/match', params).success(function (result) {
                if (result.success) {
                    gblAlrtSrv.success("Number " + group.number + " is matched to account " + result.data);
                    vm.remove(group, isCall);
                } else {
                    gblAlrtSrv.error("Failed to match");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to match");
            });
        }
    }
})();
