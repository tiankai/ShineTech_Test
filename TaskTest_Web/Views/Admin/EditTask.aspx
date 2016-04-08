<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<TaskTest_Web.Models.EditTaskModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CssJsContent" runat="server">

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div ng-app="ng_EditTask" ng-controller="editTaskCtrl" ng-init="">
        <div>
            <h2>EditTask</h2>
        </div>
        <div>
            <table class="table table-striped">
                <tr>
                    <td >
                        <%: Html.LabelFor(m => m.TaskTitle) %>
                    </td>
                    <td>
                        <%: Html.TextBoxFor(m => m.TaskTitle, new { id = 5 }) %>
                        <span>*</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.Priority) %>
                    </td>
                    <td>
                        <%: Html.DropDownListFor(m => m.TaskTitle, new SelectList(new List<TaskTest_Web.Models.TaskPriority>(){ TaskTest_Web.Models.TaskPriority.High, TaskTest_Web.Models.TaskPriority.Senior, TaskTest_Web.Models.TaskPriority.Top }, TaskTest_Web.Models.TaskPriority.Senior)) %>
                        <span>*</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.DueTime) %>
                    </td>
                    <td>
                        <%: Html.TextBoxFor(m => m.DueTime) %>
                        <span>*</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.TaskDescription) %>
                    </td>
                    <td>
                        <%: Html.TextAreaFor(m => m.TaskDescription) %>
                        <span>*</span>
                    </td>
                </tr>
                <tr>
                    <td><%: Html.HiddenFor(m => m.TaskId) %></td>
                    <td><input type="submit" value="EditTask" /></td>
                </tr>               
            </table>
        </div>
    </div>
</asp:Content>

