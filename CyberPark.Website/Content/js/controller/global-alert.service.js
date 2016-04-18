(function () {
    'use strict';

    angular
        .module('CyberPark')
        .service('GlobalAlertService', GlobalAlertService);

    GlobalAlertService.$inject = ['$rootScope'];
    function GlobalAlertService($rootScope) {
        $rootScope.globalAlerts = [];
        var service = {};
        service.success = success;
        service.error = error;
        $rootScope.closeAlert = closeAlert;

        return service;

        function initService() {
            $rootScope.$on('$locationChangeStart', function () {
                clearGlobalAlerts();
            });
        }

        function clearGlobalAlerts() {
            $rootScope.globalAlerts.length = 0;
        }

        //function successGlobal(message) {
        //    $rootScope.globalAlerts.push({
        //        message: message,
        //        type: 'success'
        //    });
            
        //}

        //function errorGlobal(message) {
        //    $rootScope.globalAlerts.push({
        //        message: message,
        //        type: 'error'
        //    });
        //}

        //function closeAlert(alert) {
        //    $rootScope.globalAlerts.splice($rootScope.globalAlerts.indexOf(alert), 1);
        //}


        function success(message, container) {
            if (!container) {
                container = $rootScope.globalAlerts;
            }
            alert(container, 'success', message);
        }

        function error(message, container) {
            if (!container) {
                container = $rootScope.globalAlerts;
            }
            alert(container, 'error', message);
        }

        function alert(container, type, message) {
            if (container) {
                container.unshift({
                    message: message,
                    type: type
                });
            }
        }

        function closeAlert(alert, container) {
            if (!container) {
                container = $rootScope.globalAlerts;
            }
            container.splice(container.indexOf(alert), 1);
        }
    }
})();