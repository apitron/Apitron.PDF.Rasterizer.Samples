var renderingController = angular.module('renderingController', []);

renderingController.controller('pageByPageRenderingController',
    ['$scope', '$http', function ($scope, $http) {

        $scope.states = { Initialized:0, Loading: 1, Displaying: 2, FailedToDisplay: 3, FailedToInitialize:4 };        

        $scope.render = function (pageIndex)
        {
            if ($scope.state==$scope.states.FailedToInitialize) {
                return;
            }
            
            $scope.imageSource = "";
            $scope.setState($scope.states.Loading);

            $http.get("/api/rendering/GetRenderedPage/" + pageIndex).success(function (data, status, headers, config)
            {                
                $scope.imageSource = data;

                if ($scope.imageSource != null && $scope.imageSource != "")
                {
                    $scope.currentPage = pageIndex;
                    $scope.setState($scope.states.Displaying);
                }
                else
                {
                    $scope.setState($scope.states.FailedToDisplay);
                }

            }).error(function (data, status, headers, config)
            {
                $scope.setState($scope.states.FailedToDisplay);
            });
        }

        $scope.renderNext = function ()
        {
            if ($scope.currentPage < $scope.maxPages - 1)
            {
                $scope.render($scope.currentPage + 1);
            }
        }

        $scope.renderPrev = function ()
        {
            if ($scope.currentPage > 0)
            {
                $scope.render($scope.currentPage - 1);
            }
        }

        $scope.loadFile = function () {
           
            $scope.currentPage = -1;

            $http.get("/api/rendering/GetFileInfo").success(function (data, status, headers, config)
            {
                $scope.maxPages = data.PageCount;

                $scope.setState($scope.states.Initialized);

                $scope.render(0);

            }).error(function (data, status, headers, config)
            {
                $scope.setState($scope.states.FailedToInitialize);
            });            
        }

        $scope.setup = function ()
        {
            $scope.setState($scope.states.FailedToInitialize);
        }

        $scope.setState = function (state) {
            $scope.state = state;            

            switch ($scope.state)
            {
                case $scope.states.Loading:
                {
                    $scope.progressBarClass = "visible";
                    $scope.pageViewClass = "invisible";
                    $scope.errorText = "";
                    break;
                }
                case $scope.states.Displaying:
                {
                    $scope.progressBarClass = "invisible";
                    $scope.pageViewClass = "visible";
                    $scope.errorText = "";
                    break;
                }
                case $scope.states.FailedToDisplay:
                    {
                        $scope.progressBarClass = "invisible";
                        $scope.pageViewClass = "invisible";
                        $scope.errorText = "Failed to render page";
                        break;
                    }
                default :
                {
                    $scope.progressBarClass = "invisible";
                    $scope.pageViewClass = "invisible";
                    $scope.errorText = "";
                    break;
                }
            }
        }

        $scope.isLoading = function ()
        {            
            return $scope.state == $scope.states.Loading;
        }

        $scope.isDisplaying = function ()
        {
            return $scope.state == $scope.states.Displaying;
        }
    }]);