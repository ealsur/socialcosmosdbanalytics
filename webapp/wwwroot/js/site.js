'use strict';

(function () {
    angular.module('Chat', [])
        .config(['$httpProvider', function ($httpProvider) {
            if (!$httpProvider.defaults.headers.get) {
                $httpProvider.defaults.headers.get = {};
            }
            $httpProvider.defaults.headers.get['If-Modified-Since'] = 'Mon, 26 Jul 1997 05:00:00 GMT';
            $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
            $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
        }])
        .controller('Comments', function ($http, $timeout, $q, $rootScope) {
            var ctrl = this;
            ctrl.posts = [];

            var timeouts = {};

            $rootScope.$on('message', function (event, data) {
                ctrl.posts.push({
                    id: data.id,
                    message: data.message,
                    hasFeedback: false,
                    score: 0
                });

                var index = ctrl.posts.length - 1;

                var check = function () {
                    $timeout(function () {
                    $http({
                        method: 'GET',
                        url: '/posts/get/' + data.id
                    }).then(function (response) {
                        if (!response.data) {
                            check();
                            return;
                        }
                        var r = ctrl.posts[index];
                        r.score = response.data.score;
                        r.hasFeedback = true;
                            });
                    }, 2000);
                }
                check();
            });

            
        })
        .controller('Poster', function ($http, $timeout, $q, $rootScope) {
            var ctrl = this;

            ctrl.message = '';
            ctrl.loading = false;
            ctrl.post = function () {
                ctrl.loading = true;
                $http({
                    method: 'POST',
                    url: '/posts/save',
                    data: {
                        message: ctrl.message
                    }
                }).then(function (response) {
                    ctrl.loading = false;
                    $rootScope.$emit('message', {
                        message: ctrl.message,
                        id: response.data
                    });
                    ctrl.message = '';
                });
            };
        })
        .controller('Metrics', function ($http, $timeout, $q) {
            var ctrl = this;

            ctrl.coreTemperature = 200;
            ctrl.heaterExchangerTemperature = 200;
            ctrl.coolantPressure = 155;

            ctrl.alert = false;

            ctrl.save = function () {
                $http({
                    method: 'POST',
                    url: '/Metrics/Save',
                    data: {
                        coreTemperature: ctrl.coreTemperature,
                        heaterExchangerTemperature: ctrl.heaterExchangerTemperature,
                        coolantPressure: ctrl.coolantPressure
                    }
                }).then(function (response) {

                    var time = 2000;
                    if (ctrl.coreTemperature > 750 && ctrl.heaterExchangerTemperature > 400 && ctrl.coolantPressure < 150) {
                        ctrl.alert = true;
                    }
                    else {
                        $timeout(function () {
                            ctrl.save();
                        }, time);
                    }
                });
            };

            $timeout(function () {
                ctrl.save();
            }, 1000);
        })
})();