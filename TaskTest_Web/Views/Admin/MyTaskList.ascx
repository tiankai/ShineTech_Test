<%@ Control Language="C#" %>

<div ng-app="myTask" ng-controller="taskCtrl" ng-init="totoalPages=0;currentPageIndex=1;pageSize=10;isFirstPage=true;isCanNext=false;isCanPrev=true;isLastPage=false;">
  
    <h2>Task</h2>
    <table  class="table table-striped">
        <thead>
            <tr>
                <th>任务标题</th>
                <th>优先级别</th>
                <th>到期时间</th>
                <th>执行者</th>
                <th>动作</th>
                <th></th>
            </tr>
        </thead>
        <tbody>
            <tr ng-repeat="t in taskList">            
                <td><span title="{{ t.memo }}">{{ t. title }}</span></td>                
                <td>{{ t.priority }}</td>
                <td>{{ t.dueTime }}</td>
                <td>{{ t.executer }}</td>
                <td>
                    <select ng-change="UserDoAction(x)" ng-model="x">
                        <option ng-repeat="item in t.doActions" ng-value="[item.doAction, t.id]">{{ item.actionMemo }}</option>
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
    app.controller('taskCtrl', function ($scope, $http, $window) {

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

        http.get("/Task/Test?pageIndex=" + pageIndex)
			.then(function (response) {

			    scope.taskList = response.data.Tasks;
			    scope.totoalPages = response.data.PagePart.TotoalPages;
			    scope.currentPageIndex = response.data.PagePart.PageIndex;
			    scope.pageSize = response.data.PagePart.PageSize;

			    if (scope.currentPageIndex == 1) { // 第一页

			        scope.isLastPage = false;
			        scope.isFirstPage = true;
			        scope.isCanPrev = true;
			        if (scope.totoalPages >= 2)
			        {
			            scope.isCanNext = false;                        
			        }
			        else
			        {
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
			        else
			        {
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
