<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<TaskTest_Web.Models.NewTaskModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssJsContent" runat="server">

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
    <%: Html.ValidationSummary(true, "Create Task failed, Please correct the errors and try again." ) %>

    <% using (Html.BeginForm("NewTask", "Admin")) { %>
    <fieldset>
        <legend>
            Create New Task
        </legend>
        <ol>
            <li>
                <%: Html.LabelFor(m => m.TaskTitle) %>
                <%: Html.TextBoxFor(m => m.TaskTitle) %>
                <%: Html.ValidationMessageFor(m => m.TaskTitle) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.Priority) %>
                <%: Html.DropDownListFor(m => m.Priority, new SelectList(new List<TaskTest_Web.Models.TaskPriority>(){ TaskTest_Web.Models.TaskPriority.High, TaskTest_Web.Models.TaskPriority.Senior, TaskTest_Web.Models.TaskPriority.Top }, TaskTest_Web.Models.TaskPriority.Senior)) %>                
            </li>
            <li>
                <%: Html.LabelFor(m => m.DueTime) %>
                <%: Html.TextBoxFor(m => m.DueTime) %>
                <%: Html.ValidationMessageFor(m => m.DueTime) %>
            </li>
            <li>
                <%: Html.LabelFor(m => m.TaskDescription) %>
                <%: Html.TextAreaFor(m => m.TaskDescription) %>
            </li>
        </ol>
        <input type="submit" value="New Task" title="New Task" />
        <%: Html.HiddenFor(m => m.ParentTaskId) %>
    </fieldset>
    <% } %>
    </div>
</asp:Content>
