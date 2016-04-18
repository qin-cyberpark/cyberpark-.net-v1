(function () {
    'use strict';

    angular
        .module('CyberPark')
        .controller('LoginController', LoginController);

    LoginController.$inject = ['$http', 'GlobalAlertService'];
    function LoginController($http, gblAlrtSrv) {
        var vm = this;
        //login
        vm.login = function () {
            vm.submitting = true;
            $http.post('/api/login', { 'PhoneOrEmail': vm.phoneOrEmail, 'Password': vm.password }).success(function (result) {
                if (result.success) {
                    //redirect
                    window.location.href = "/";
                } else {
                    vm.submitting = false;
                    gblAlrtSrv.error("Mobile or password is incorrect");
                }
            }).error(function () {
                vm.submitting = false;
                gblAlrtSrv.error("Failed to sign in");
            });
        };

        //logout
        vm.logout = function () {
            console.log("logout");
            $http.post('/api/logout', null).success(function () {
                window.location.href = "/";
            });
        };
    };
})();