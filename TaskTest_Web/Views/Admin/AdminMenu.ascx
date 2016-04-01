<%@ Import Namespace="TaskTest_Web.Models" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<AdminMenusModel>" ClassName="AdminMenu"  %>


<div class="well sidebar-nav">
    <ul class="nav nav-list">
    <%
        if (Model != null)
        {
            if (Model.MainMenus != null)
            {
                foreach (var mainMenu in Model.MainMenus)
                {
                %>
                    <li class="nav-header">
                        <%: mainMenu.MenuName %>
                    </li>                   
                <%
                    if (Model.ChildMenus != null)
                    {
                        foreach (var subMenu in Model.ChildMenus)
                        {
                            if (subMenu.parentId == mainMenu.MenuId)
                            {
                                if(subMenu.ActionName.Equals(Model.selectedAction, StringComparison.CurrentCultureIgnoreCase) == true)
                                {
                            %>
                                    <li class="active"><%: Html.ActionLink(subMenu.MenuName, subMenu.ActionName, subMenu.ControllerName)%></li>
                            <%  
                                }
                                else // 与选择的Action不同
                                {
                            %>
                                    <li><%: Html.ActionLink(subMenu.MenuName, subMenu.ActionName, subMenu.ControllerName)%></li>
                            <%
                                }
                            }
                        }
                    }
                }
            }                      
            if(Model.AdminMenus != null)
            {
                foreach (var subAdminMenu in Model.AdminMenus)
                {
                    if(subAdminMenu.ActionName.Equals(Model.selectedAction, StringComparison.CurrentCultureIgnoreCase) == true)
                    {                       
                    %>
                        <li class="active">
                            <%: Html.ActionLink(subAdminMenu.MenuName, subAdminMenu.ActionName, subAdminMenu.ControllerName) %>                            
                        </li>
                    <%  
                    }
                    else
                    {                        
                    %>
                        <li>
                            <%: Html.ActionLink(subAdminMenu.MenuName, subAdminMenu.ActionName, subAdminMenu.ControllerName) %>
                        </li>
                    <% 
                    }                              
                }
            }           
        }                      
    %>
    </ul>
</div>