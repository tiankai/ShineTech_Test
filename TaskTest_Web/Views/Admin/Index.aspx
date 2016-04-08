<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Admin.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CssJsContent" runat="server">

    <link rel="stylesheet" href="<%: Url.Content("~/Content/TaskTree.css") %>" />
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("MyTaskList"); %>

</asp:Content>
