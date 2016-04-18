(function () {
    'use strict';

    angular
        .module('CyberPark')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$http', 'GlobalAlertService'];
    function LoginController($http, gblAlrtSrv) {
        var vm = this;
        vm.newCustomer = {
            firstName: "",
            lastName:"",
            email: "",
            mobile: "",
            password: "",
            confirmPassword:""
        }

        //login
        vm.login = function (returnUrl) {
            vm.signinging = true;
            $http.post('/api/login', { 'PhoneOrEmail': vm.phoneOrEmail, 'Password': vm.password }).success(function (result) {
                if (result.success) {
                    //redirect
                    window.location.href = returnUrl;
                } else {
                    gblAlrtSrv.error("Username or password is incorrect");
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to sign in");
            }).finally(function () {
                vm.signinging = false;
            });;
        };

        //logout
        vm.logout = function () {
            $http.post('/api/logout', null).success(function () {
                window.location.href = "/";
            }).error(function () {
                console.log("error");
            });
        };

        //signup
        vm.signup = function (returnUrl) {
            vm.signuping = true;
            console.log(vm.newCustomer.confirmPassword, vm.newCustomer.password);
            if (vm.newCustomer.confirmPassword !== vm.newCustomer.password) {
                gblAlrtSrv.error("twice password are different");
                vm.signuping = false;
                return;
            }

            $http.post('/api/signup', vm.newCustomer).success(function (result) {
                if (result.success) {
                    //redirect
                    window.location.href = returnUrl;
                } else {
                    gblAlrtSrv.error("Failed to sign up." + result.message);
                }
            }).error(function () {
                gblAlrtSrv.error("Failed to sign up");
            }).finally(function () {
                vm.signuping = false;
            });
        }
    };
})();