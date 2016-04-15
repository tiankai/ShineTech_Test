<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<TaskTest_Web.Models.NewTaskModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssJsContent" runat="server">

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/xRule.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {

            $('#TaskTitle').bind('blur', function () {

                var url = "/Task/ValidateName?name=" + $(this).val();
                // 
                $.get(url, function (data) {

                    $("#taskTitleIsExists").val(data);
                });
            });

            $.validator.addMethod("TitleUniqueCheck", function () {

                var isExist = parseInt($('#taskTitleIsExists').val());
                if(isExist == 0)
                {
                    return true;
                }
                else
                {
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

            $('#NewTaskForm').validate({
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
    <%: Html.ValidationSummary(true, "Create Task failed, Please correct the errors and try again." ) %>

    <% using (Html.BeginForm("NewTask", "Admin", FormMethod.Post, new { @id = "NewTaskForm" }))
       { %>
    <fieldset>
        <legend>
            Create <%: Model.IsTopTask ? "TopNode Task" : "SubNode Task Depends on a Task"%>
        </legend>
        <ul>
            <li>
                <%: Html.LabelFor(m => m.TaskTitle)%>
                <%: Html.TextBoxFor(m => m.TaskTitle, new { @class = "TaskTitleLengthCheck TitleUniqueCheck" })%>
                <%: Html.ValidationMessageFor(m => m.TaskTitle)%>
            </li>
            <li>
                <%: Html.LabelFor(m => m.Priority)%>
                <%: Html.DropDownListFor(m => m.Priority,
                        new SelectList(
                            new List<TaskTest_Web.Models.TaskPriority>(){
                                TaskTest_Web.Models.TaskPriority.High, 
                                TaskTest_Web.Models.TaskPriority.Senior, 
                                TaskTest_Web.Models.TaskPriority.Top,
                                TaskTest_Web.Models.TaskPriority.Junior,
                                TaskTest_Web.Models.TaskPriority.Low,
                                TaskTest_Web.Models.TaskPriority.None
                            }, TaskTest_Web.Models.TaskPriority.High
                        )
                    )%>                
            </li>
            <li>
                <%: Html.LabelFor(m => m.DueTime)%>
                <%: Html.EditorFor(m => m.DueTime, new { @type = "date" })%>
                <%: Html.ValidationMessageFor(m => m.DueTime)%>
            </li>
            <li>
                <%: Html.LabelFor(m => m.TaskDescription)%>
                <%: Html.TextAreaFor(m => m.TaskDescription, new { @class = "TaskDesLengthCheck" })%>
                <%: Html.ValidationMessageFor(m => m.TaskDescription) %>
            </li>
        </ul>
        <input type="submit" value="New Task" title="New Task" />
        <input id="taskTitleIsExists" type="hidden" value="0" />
        <%: Html.HiddenFor(m => m.ParentTaskId)%>
    </fieldset>
    <% } %>
    </div>
</asp:Content>
