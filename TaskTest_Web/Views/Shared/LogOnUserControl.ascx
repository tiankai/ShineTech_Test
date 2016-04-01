<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<% 
    const string keyName = "UName";
    var cookie = HttpContext.Current.Request.Cookies[keyName];
    if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
    {
        string hashName = cookie.Value;
        var user = HttpContext.Current.Session[hashName] as TaskTest_Web.Models.LogOnModel;
        if (user == null)
        {
%>
            <ul>
                <li>
                    <%: Html.ActionLink("Register", "Register", "Account", routeValues: null, htmlAttributes: new { id = "registerLink", data_dialog_title = "Registration" })%>
                </li>
                <li>
                    <%: Html.ActionLink("Log On", "LogOn", "Account", routeValues: null, htmlAttributes: new { id = "logonLink", data_dialog_title = "Identification" })%>
                </li>
            </ul>
<%
        }
        else
        {
%>
            <p>
                Hello,
                <%: Html.ActionLink(user.UserName, "ChangePassword", "Account", null, new { @class = "username" })%>!
                <%: Html.ActionLink("Log Off", "LogOff", "Account")%>
            </p>
<%
        }
    }
    else
    {
%>
        <ul>
            <li>
                <%: Html.ActionLink("Register", "Register", "Account", routeValues: null, htmlAttributes: new { id = "registerLink", data_dialog_title = "Registration" })%>
            </li>
            <li>
                <%: Html.ActionLink("Log On", "LogOn", "Account", routeValues: null, htmlAttributes: new { id = "logonLink", data_dialog_title = "Identification" })%>
            </li>
        </ul>
<%
    }
%>