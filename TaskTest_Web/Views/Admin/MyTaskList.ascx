<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TaskTest_Web.Models.TaskListViewModel>" %>

<div ng-app="myTask" ng-controller="taskCtrl" ng-init="totoalPages=0;currentPageIndex=3;pageSize=10;isCanUp=false;isCanDown=true">
    
    <h2>Task</h2>
    <div>data: {{ content }}</div>
    <table  class="table table-striped">
        <thead>
            <tr>
                <th>任务标题</th>
                <th>优先级别</th>
                <th>到期时间</th>
                <th>执行者</th>
            </tr>
        </thead>
        <tbody>
            <tr ng-repeat="t in taskList">
                <td>s</td>
                <td><span onclick="getSubTasks(20);">{{ t.title }}</span></td>
                <td>{{ t.priority }}</td>
                <td>{{ t.dueTime }}</td>
                <td>{{ t.executer }}</td>
            </tr>
        </tbody>
    </table>
    <div class="container">    
            <span>共计[{{ totoalPages }}]页, 当前[{{ currentPageIndex }}]页, 每页[{{ pageSize }}]记录</span>
            <a id="Pager_HeadLink" href="" >首 页</a>
            <a id="Pager_PreviousLink" href="" >上一页</a>
            <a id="Pager_NextLink" href="" >下一页</a>
            <a id="Pager_EndLink" href="" >末 页</a>
    </div>
</div>
<div id="TaskTree">
    <div>hello sofas</div>
    <h2>asdlfjasdoiwejjkflasjfl;kkkkhhhiuggygyggjhgjhgjhjhjhjhjhjhjhjhjhjhjcccccfhf</h2>
    <h3>ow emcps lsod </h3>
</div>
<script type="text/html">

</script>
<script>
    var app = angular.module('myTask', []);
    app.controller('taskCtrl', function ($scope, $http) {

        $http.get("/Task/Test")
			.then(function (response) {

			    $scope.content = response.data;
			    $scope.taskList = response.data.Tasks;
			    $scope.totoalPages = response.data.PagePart.TotoalPages;
			    $scope.currentPageIndex = response.data.PagePart.PageIndex;
			    $scope.pageSize = response.data.PagePart.PageSize;

			    if ($scope.currentPageIndex == 1) {

			        $("#Pager_HeadLink").click(function () { return false; });
			        $("#Pager_PreviousLink").attr('disabled', true);
			        $("#Pager_NextLink").attr('disabled', false);
			        $("#Pager_EndLink").click(function () { alert("End Link"); return true; });
			    }

			    if ($scope.currentPageIndex == $scope.totoalPages) {

			        $("#Pager_HeadLink").attr('disabled', false);
			        $("#Pager_PreviousLink").attr('disabled', false);
			        $("#Pager_NextLink").attr('disabled', true);
			        $("#Pager_EndLink").attr('disabled', true);
			    }



			});
    });

    function pageQuery(pageIndex) {

        alert("Hello");
    }

    function getSubTasks(taskid) {

        var treeDiv = $("#TaskTree");
        treeDiv.css('display', 'block');
        treeDiv.css('top', '-130px');

    }

</script>
