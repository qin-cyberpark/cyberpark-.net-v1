(function () {
    'use strict';

    var module = angular.module('CyberPark');
    module.controller('ProductController', ProductController);
    module.factory('$productEditModal', function () {
        var modal = $('#productEditModal');
        return modal;
    });

    ProductController.$inject = ['$scope', '$http', 'GlobalAlertService', '$productEditModal'];
    function ProductController($scope, $http, gblAlrtSrv, $editModal) {
        /* jshint validthis:true */

        var vm = this;
        vm.products = [];
        vm.product = {};
        vm.editingProduct = {}
        vm.alerts = [];
        $scope.$on('account.loaded', function (event, accountId) {
            vm.load(accountId);
        });

        vm.init = function (rptType) {
            vm.repeatType = rptType;
        }

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

        /* load products*/
        vm.load = function (accountId) {
            //load account
            $http.get('/api/account/' + accountId + '/Product/', { params: { repeatTypes: vm.repeatType } }).success(function (result) {
                if (result.success) {
                    //show account
                    vm.products = result.data;
                } else {
                    gblAlrtSrv.error("Failed to load products");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to load products");
            });
        }

        //convert date
        vm.convertDate = function (product) {
            //convert date 
            product.serviceGivenDate = new Date(product.serviceGivenDate);
            product.chargedToDate = new Date(product.chargedToDate);
            product.termStartDate = new Date(product.termStartDate);
            product.oneOffChargeDate = new Date(product.oneOffChargeDate);
            $.each(product.services, function (idx, srv) {
                srv.readyForServiceDate = new Date(srv.readyForServiceDate);
            });
            $.each(product.services, function (idx, srv) {
                srv.readyForServiceDate = new Date(srv.readyForServiceDate);
            });
            return product;
        }

        /* open edit product modal */
        vm.edit = function (product) {
            vm.editingProduct = angular.copy(vm.convertDate(product));
            vm.alerts.length = 0;
            $editModal.modal('show');
        }

        /* save general */
        vm.saveGeneral = function () {
            var editingGeneral = {};
            angular.extend(editingGeneral, vm.editingProduct);
            //update general
            $http.put('/api/product/', editingGeneral).success(function (result) {
                if (result.success) {
                    vm.load(result.data.accountId);
                    vm.success("product general infomation updated");
                } else {
                    vm.error("Failed to updated product general infomation");
                }
            }).error(function () {
                vm.error("Failed to updated product general infomation");
            });
        }

        /* add service */
        vm.addService = function (type, subType) {
            vm.editingProduct.services.push({
                productId: vm.editingProduct.id,
                type: type,
                subType: subType,
                status: 'Using',
                isDeleted:false
            });
            console.log(vm.editingProduct.services);
        }

        /* remove service */
        vm.removeService = function (srv) {
            if (!srv.id){
                vm.editingProduct.services.splice(vm.editingProduct.services.indexOf(srv), 1);
            }else {
                $http.delete('/api/service/' + srv.id).success(function (result) {
                    if (result.success) {
                        vm.editingProduct.services.splice(vm.editingProduct.services.indexOf(srv), 1);
                        vm.load(vm.editingProduct.accountId);
                        vm.success("service deleted");
                    } else {
                        vm.error("Failed to delete service");
                    }
                }).error(function () {
                    vm.error("Failed to delete service");
                });
            }
        }

        /* save service */
        vm.saveService = function (srv) {
            console.log("Save");
            //update general
            if (srv.id) {
                $http.put('/api/service/', srv).success(function (result) {
                    if (result.success) {
                        srv = result.data;
                        vm.load(vm.editingProduct.accountId);
                        vm.success("service updated");
                    } else {
                        vm.error("Failed to updated service");
                    }
                }).error(function () {
                    vm.error("Failed to updated service");
                });
            } else {
                //new save
                $http.post('/api/service/', srv).success(function (result) {
                    if (result.success) {
                        vm.load(vm.editingProduct.accountId);
                        vm.success("service added");
                    } else {
                        vm.error("Failed to add service");
                    }
                }).error(function () {
                    vm.error("Failed to add service");
                });
            }
        }

        /* add offer */
        vm.addOffer = function (type, subType) {
            vm.editingProduct.usageOffers.push({
                productId: vm.editingProduct.id,
                serviceType: type,
                serviceSubType: subType,
                minutes: 200,
                isDeleted: false
            });
        }

        /* remove offer */
        vm.removeOffer = function (offer) {
            if (!offer.id) {
                vm.editingProduct.usageOffers.splice(vm.editingProduct.usageOffers.indexOf(offer), 1);
            }else{
                $http.delete('/api/UsageOffer/'+ offer.id).success(function (result) {
                    if (result.success) {
                        vm.editingProduct.usageOffers.splice(vm.editingProduct.usageOffers.indexOf(offer), 1);
                        vm.load(vm.editingProduct.accountId);
                        vm.success("calling offer deleted");
                    } else {
                        vm.error("Failed to delete calling offer");
                    }
                }).error(function () {
                    vm.error("Failed to delete calling offer");
                });
            }
        }

        /* save offer */
        vm.saveOffer = function (offer) {
            //update general
            if (offer.id) {
                $http.put('/api/UsageOffer/', offer).success(function (result) {
                    if (result.success) {
                        offer = result.data;
                        vm.load(vm.editingProduct.accountId);
                        vm.success("calling offer updated");
                    } else {
                        vm.error("Failed to update calling offer");
                    }
                }).error(function () {
                    vm.error("Failed to update calling offer");
                });
            } else {
                //new save
                $http.post('/api/UsageOffer/', offer).success(function (result) {
                    if (result.success) {
                        vm.load(vm.editingProduct.accountId);
                        vm.success("service added");
                    } else {
                        vm.error("Failed to add calling offer");
                    }
                }).error(function () {
                    vm.error("Failed to add calling offer");
                });
            }
        }
    }
})();