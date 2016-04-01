using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskTest_Web.Models
{
    public class AdminMenusModel
    {
        public IList<MainMenuInfo> MainMenus { get; set; }

        public IEnumerable<SubMenuInfo> ChildMenus { get; set; }

        public IEnumerable<SubMenuInfo> AdminMenus { get; set; }

        public string selectedAction { get; set; }
    }

    public class MainMenuInfo
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }
    }

    public class SubMenuInfo
    {
        public string MenuName { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public int parentId { get; set; }
    }

    public interface IMenuQuery
    {
        IList<MainMenuInfo> QueryTopMenus();

        IEnumerable<SubMenuInfo> QuerySubMenus(int parentId);

        IEnumerable<SubMenuInfo> QuerySubMenus(int parentId, int userId);
    }

}