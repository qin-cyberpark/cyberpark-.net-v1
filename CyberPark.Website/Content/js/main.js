(function () {
    'use strict';

    var module = angular.module('CyberPark', ['angular-ladda']);
    module.directive('icheck', function ($timeout, $parse) {
        return {
            require: 'ngModel',
            link: function ($scope, element, $attrs, ngModel) {
                return $timeout(function () {
                    var value = $attrs['value'];
                    $scope.$watch($attrs['ngModel'], function (newValue) {
                        $(element).iCheck('update');
                    })

                    $scope.$watch($attrs['ngDisabled'], function (newValue) {
                        $(element).iCheck(newValue ? 'disable' : 'enable');
                        $(element).iCheck('update');
                    })

                    return $(element).iCheck({
                        checkboxClass: 'icheckbox_square-green',
                        radioClass: 'iradio_square-green'
                    }).on('ifToggled', function (event) {
                        if ($(element).attr('type') === 'checkbox' && $attrs['ngModel']) {
                            $scope.$apply(function () {
                                return ngModel.$setViewValue(event.target.checked);
                            });
                        }
                        if ($(element).attr('type') === 'radio' && $attrs['ngModel']) {
                            return $scope.$apply(function () {
                                return ngModel.$setViewValue(value);
                            });
                        }
                    });
                });
            }
        };
    });

})();

(function ($) {
	$.scrollUp({
        scrollName: 'scrollUp', // Element ID
        topDistance: '300', // Distance from top before showing element (px)
        topSpeed: 300, // Speed back to top (ms)
        animation: 'fade', // Fade, slide, none
        animationInSpeed: 200, // Animation in speed (ms)
        animationOutSpeed: 200, // Animation out speed (ms)
        scrollText: '', // Text for element
        activeOverlay: false // Set CSS color to display scrollUp active point, e.g '#00FFFF'
	});


	//$('input[type=radio]').iCheck({ checkboxClass: 'icheckbox_square-green', radioClass: 'iradio_square-green' });
	$('input[type=checkbox]').iCheck({ checkboxClass: 'icheckbox_square-green', radioClass: 'iradio_square-green' });

})(jQuery);