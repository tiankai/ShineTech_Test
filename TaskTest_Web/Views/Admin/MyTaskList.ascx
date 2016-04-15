<%@ Control Language="C#" %>

<div ng-app="myTask" ng-controller="taskCtrl" ng-init="totoalPages=0;currentPageIndex=1;pageSize=10;isFirstPage=true;isCanNext=false;isCanPrev=true;isLastPage=false;">
  
    <h2>Task List</h2>
    <table  class="table table-striped">
        <thead>
            <tr>
                <th>标 题</th>
                <th ng-click="orderFunction('Priority')">级 别</th>
                <th>时 间</th>
                <th>创建者</th>
                <th>执行者</th>
                <th>进 度</th>
                <th ng-click="orderFunction('Status')">状 态</th>
                <th>动 作</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <tr ng-repeat="t in taskList | orderBy : orderTaskBy ">            
                <td><span title="{{ t.memo }}">{{ t. TaskTitle | titleFormat }}</span></td>                
                <td>{{ t.Priority }}</td>
                <td>
                    {{ t.CreateTime }}
                </td>
                <td>Knight</td>
                <td>{{ t.Executer | lowercase }}</td>
                <td>{{ t.Executer | xDateFormat }}</td>
                <td>{{ t.Status }}</td>
                <td>
                    <select class="selectCtrl" ng-change="UserDoAction(x)" ng-model="x">
                        <option ng-repeat="item in t.UserActions" ng-value="[item.ActionType, t.id]">{{ item.ActionTitle }}</option>
                    </select>                    
                </td>
                <td>
                    <span onclick="getTaskTreeDialog(this.title, 'Task - Tree', 750);" title="{{ t.id }}">Task Tree</span>
                </td>
            </tr>
        </tbody>
    </table>
    <div class="container">    
            <span>共计[{{ totoalPages }}]页, 当前第[{{ currentPageIndex }}]页, 每页[{{ pageSize }}]记录</span>
            <button id="Pager_HeadLink" ng-disabled="isFirstPage" ng-click="HeadPageQuery()">首 页</button>
            <button id="Pager_PreviousLink" ng-disabled="isCanPrev" ng-click="PrevPageQuery()">上一页</button>
            <button id="Pager_NextLink" ng-disabled="isCanNext" ng-click="NextPageQuery()">下一页</button>
            <button id="Pager_EndLink" ng-disabled="isLastPage" ng-click="TailPageQuery()">末 页</button>
    </div>
</div>
<script>
    var app = angular.module('myTask', []);

    app.filter('titleFormat', function () {

        return function (title) {

            if(title.length > 12)
            {
                return "for short....";
            }
            else
            {
                return title;
            }
        };
    });

    app.filter('xDateFormat', function () {

        return function (x) {

            if (x.length > 0) {
                return x;
            }
            else
            {
                return "null";
            }
        };
    });

    app.controller('taskCtrl', function ($scope, $http, $window) {

        $scope.orderFunction = function (x) {

            $scope.orderTaskBy = x;
        };

        GetTaskList($scope, $http, 1);

        $scope.UserDoAction = function (x) {

            //alert("len = " + x.length + ", doAction = " + x[0] + ", TaskId = " + x[2]);
            
            userDoAction(x[0], x[2], $http, $scope, $window);
        };

        $scope.HeadPageQuery = function () {

            GetTaskList($scope, $http, 1);
        };

        $scope.NextPageQuery = function () {

            GetTaskList($scope, $http, $scope.currentPageIndex + 1);
        };

        $scope.PrevPageQuery = function () {

            GetTaskList($scope, $http, $scope.currentPageIndex - 1);
        };

        $scope.TailPageQuery = function () {

            GetTaskList($scope, $http, $scope.totoalPages);
        };
    });
    
    function GetTaskList(scope, http, pageIndex) {

        http.get("/Task/GetTaskList?pageIndex=" + pageIndex)
			.then(function (response) {

			    if (response.data.result == 0)
			    {
			        scope.taskList = response.data.TaskList;
			        scope.totoalPages = response.data.TaskTotalPages;
			        scope.currentPageIndex = response.data.CurrentPageIndex;
			        scope.pageSize = response.data.TaskPageCount;

			        if (scope.currentPageIndex == 1) { // 第一页

			            scope.isLastPage = false;
			            scope.isFirstPage = true;
			            scope.isCanPrev = true;
			            if (scope.totoalPages >= 2) {
			                scope.isCanNext = false;
			            }
			            else {
			                scope.isCanNext = true;
			            }
			        }
			        else if (scope.currentPageIndex == scope.totoalPages) { // 最后一页

			            scope.isLastPage = true;
			            scope.isFirstPage = false;
			            scope.isCanNext = true;
			            if (scope.totoalPages >= 2) {
			                scope.isCanPrev = false;
			            }
			            else {
			                scope.isCanPrev = true;
			            }
			        }
			        else  // 中间页
			        {
			            scope.isLastPage = false;
			            scope.isFirstPage = false;
			            scope.isCanNext = false;
			            scope.isCanPrev = false;
			        }
			    }
			    else if(response.data.result == 1)
			    {
			        alert("user = null !");
			    }
			    else if(response.data.result == 2)
			    {
			        alert("error took place");
			    }
			    else 
			    {
			        alert("data = " + response.data.result);
			    }
			    
			});

    }
    
    function userDoAction(doAction, taskId, http, scope, window) {

        var baseUrl = 'http://' + window.location.host;
        if (doAction == 0) // 新建任务
        {     
            window.location.href = baseUrl + '/Admin/NewTask?id=' + taskId;
        }
        else if (doAction == 1)  // 分配任务
        {
            var title = "Users for Select";
            var width = 450;
            // 用户对话框id
            var dialogId = "Users_" + taskId;
            // 获取视图的url地址
            var url = "/Admin/GetTaskTree?id=" + taskId;
            // 
            if (!dialogs[dialogId]) {
                loadAndShowDialog(dialogId, url, title, width);
            } else {
                dialogs[dialogId].dialog('open');
            }
        }
        else if (doAction == 2) // 受理任务
        {
            http.get("/Task/AcceptTask?id=" + taskId)
                .then(function (response) {

                    var res = response.data.result;
                    var mess = response.data.message;

                    alert(" result = " + res +", message = " + mess);
                });

        }
        else if (doAction == 3) // 拒绝任务
        {
            http.get("/Task/RejectTask?id=" + taskId)
                .then(function (response) {

                    var res = response.data.result;
                    var mess = response.data.message;

                    alert(" result = " + res + ", message = " + mess);
                });
        }
        else if (doAction == 4) // 完结任务
        {
            http.get("/Task/TaskDone?id=" + taskId)
                .then(function (response) {

                    var res = response.data.result;
                    var mess = response.data.message;

                    alert(" result = " + res + ", message = " + mess);
                });
        }
        else if (doAction == 5) // 编辑任务
        {
            window.location.href = baseUrl + '/Admin/EditTask?id=' + taskId;
        }
    }
    
</script>
