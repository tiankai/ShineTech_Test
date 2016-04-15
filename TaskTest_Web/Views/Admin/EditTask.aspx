<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<TaskTest_Web.Models.EditTaskModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="CssJsContent" runat="server">
    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/xRule.js") %>" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            $('#TaskTitle').bind('blur', function () {

                var newName = $(this).val();
                var oldName = $('#hd_TaskTitle').val();

                if (oldName == newName)
                {
                    $("#taskTitleIsExists").val('0');
                }
                else
                {
                    var url = "/Task/ValidateName?name=" + newName;
                    // 
                    $.get(url, function (data) {

                        $("#taskTitleIsExists").val(data);
                    });
                }                
            });

            $.validator.addMethod("TitleUniqueCheck", function () {

                var isExist = parseInt($('#taskTitleIsExists').val());
                if (isExist == 0) {
                    return true;
                }
                else {
                    return false;
                }

            }, "Task Title already exists!");

            $.validator.addMethod("TaskTitleLengthCheck", function (value, element) {

                var len = getByteLen(value);
                if (len > 40) {
                    return false;
                }
                else {
                    return true;
                }
            }, "Task Title exceeds 40 bytes!");

            $.validator.addMethod("TaskDesLengthCheck", function (value, element) {

                var len = getByteLen(value);
                if (len > 512) {
                    return false;
                }
                else {
                    return true;
                }
            }, "Task Description exceeds 512 bytes!");

            $('#EditTaskForm').validate({
                rules: {
                    TaskTitle: {
                        TaskTitleLengthCheck: true,
                        TitleUniqueCheck: true
                    },
                    TaskDescription: {
                        TaskDesLengthCheck: true
                    }
                }
            });
        });
    </script>
    <div>
        <%: Html.ValidationSummary(true, "Edit Task failed, Please correct the errors and try again." ) %>

        <% using (Html.BeginForm("EditTask", "Admin", FormMethod.Post, new { @id = "EditTaskForm" }))
           { %>
        <div>
            <h2>Edit Task</h2>
        </div>
        <div>
            <table class="table table-striped">
                <tr>
                    <td >
                        <%: Html.LabelFor(m => m.TaskTitle)%>
                    </td>
                    <td>
                        <%: Html.HiddenFor(m => m.TaskTitle, new { @id = "hd_TaskTitle" })%>
                        <%: Html.TextBoxFor(m => m.TaskTitle, new { @class = "TaskTitleLengthCheck TitleUniqueCheck" })%>
                        <%: Html.ValidationMessageFor(m => m.TaskTitle)%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.Priority)%>
                    </td>
                    <td>
                        <%: Html.DropDownListFor(m => m.Priority, new SelectList(
                                new List<TaskTest_Web.Models.TaskPriority>(){ 
                                    TaskTest_Web.Models.TaskPriority.High, 
                                TaskTest_Web.Models.TaskPriority.Senior, 
                                TaskTest_Web.Models.TaskPriority.Top,
                                TaskTest_Web.Models.TaskPriority.Junior,
                                TaskTest_Web.Models.TaskPriority.Low,
                                TaskTest_Web.Models.TaskPriority.None
                                }, Model.Priority
                            ))%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.DueTime)%>
                    </td>
                    <td>
                        <%: Html.EditorFor(m => m.DueTime, new { @type = "date" })%>
                        <%: Html.ValidationMessageFor(m => m.DueTime)%>
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.TaskDescription)%>
                    </td>
                    <td>
                        <%: Html.TextAreaFor(m => m.TaskDescription, new { @class = "TaskDesLengthCheck" })%>         
                        <%: Html.ValidationMessageFor(m => m.TaskDescription)%>               
                    </td>
                </tr>
                <tr>
                    <td>
                        <%: Html.HiddenFor(m => m.TaskId)%>
                        <input id="taskTitleIsExists" type="hidden" value="0" />
                    </td>
                    <td><input type="submit" value="EditTask" /></td>
                </tr>               
            </table>
        </div>
        <% } %>
    </div>
</asp:Content>

