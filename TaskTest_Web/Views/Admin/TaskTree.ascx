<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TaskTest_Web.Models.TaskTreeNode>" %>

<%
    if(Model.NodeLevel > 0) // 不是根节点
    {
%>
        <div class="leftFlag">
            |
        </div>
<%
    }
%>
        <!-- 节点 -->
        <div class="TaskRootNode">
<%
    // 判断是第几层节点
    for(int i = 0;i < Model.NodeLevel;i++)
    {
%>
            <div class="leftGap">
                -
            </div>
<%            
    }
    //                
    if(Model.NodeLevel > 0)
    {
        if(Model.NodeLevel % 2 == 0)  // 偶数行节点
        {
%>
            <div class="evenSubNode">   
<%                
        }
        else // 奇数行节点
        {
%>
            <div class="oddSubNode">   
<%              
        }
    }
    else // 根节点
    {
%>
            <div class="rootNodeText">   
<%          
    }
%>
            <!-- 节点详细信息 -->
                <div class="eachItem">
                    <!-- 任务标题和说明 -->
                    <span title="<%: Model.RootNode.TaskDescription %>"><%: Model.RootNode.TaskTitle %></span>
                </div>
                <!-- 任务优先级 -->
                <div class="eachItem"><%: Model.RootNode.Priority %></div>
                <!-- 任务状态 -->
                <div class="eachItem"><%: Model.RootNode.Status %></div>
                <!-- 创建人 -->
                <div class="eachItem"><%: Model.RootNode.Creater %></div>
                <!-- 时间信息  创建 分配 受理 完结.... -->
                <div class="eachItem"></div>
            </div>
            <br />
        </div>
<%
    if(Model.ChildrenNodes != null && Model.ChildrenNodes.Count() > 0)
    {
%>
        <!-- 节点的子节点信息 --> 
        <div class="TaskChildrenNodes">
            <%
                foreach(var eachChildNode in Model.ChildrenNodes)
                {
                    Html.RenderPartial("TaskTree", eachChildNode);              
                }
            %>
        </div>
<%
    }
%>
