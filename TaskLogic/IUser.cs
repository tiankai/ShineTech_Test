using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskLogic
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum UserRole : uint
    {
        /// <summary>
        /// 超级管理员
        /// </summary>
        AdminUser = 0,
        /// <summary>
        /// 组员
        /// </summary>
        NormalUser = 1,
        /// <summary>
        /// 组长
        /// </summary>
        GroupUser,
        /// <summary>
        /// 主管
        /// </summary>
        TeamLeader,
        /// <summary>
        /// 高管
        /// </summary>
        Exectuer
    }
    /// <summary>
    /// 用户结构
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPass { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 电子邮件地址
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public UserRole Actor { get; set; }
    }

    public interface IUser
    {
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        UserInfo UserLogin(string userName, string userPass);
        /// <summary>
        /// 用户退出
        /// </summary>
        /// <param name="userName"></param>
        void UserLogout(string userName);
        /// <summary>
        /// 注册用户信息
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns></returns>
        bool RegisterUser(UserInfo user);
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="oldPass">原密码</param>
        /// <param name="newPass">新密码</param>
        /// <returns></returns>
        bool ModifyUserPass(string userName, string oldPass, string newPass);

    }
}
