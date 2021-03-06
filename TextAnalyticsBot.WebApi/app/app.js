﻿"use strict"

angular.module("TextAnalyticsBot", ["ui.bootstrap", "angular-loading-bar"])

.service("AppService", ["$http", function ($http) {
    this.postMessage = function (message) {
        return $http({
            method: 'POST',
            url: 'api/messages',
            headers: { 'Content-Type': 'application/json' },
            data: JSON.stringify(message)
        }).success(function (data) {
        }).error(function (data) {
            console.log(data);
            alert(data.Message);
        });
    };
}])

.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if(event.which === 13) {
                scope.$apply(function (){
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
})

.controller("AppController", ["$scope", "AppService", function ($scope, AppService) {
    
}]);