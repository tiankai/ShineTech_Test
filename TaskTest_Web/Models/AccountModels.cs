using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskTest_Web.Models
{
    /// <summary>
    /// 用户认证
    /// </summary>
    public interface IUserAuth
    {
        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        int UserLogin(LogOnModel model);
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool RegisterUser(RegisterModel model);
        /// <summary>
        /// 查找用户列表
        /// </summary>
        /// <param name="userName">null or empty 返回所有用户, 不为空返回模糊匹配的用户</param>
        /// <returns></returns>
        UserListViewModel GetUserList(string userName);
    }

    public class UserAuth : IUserAuth
    {
        private static UserAuth _userAuth;

        private UserAuth()
        {
        }

        public static UserAuth GetInstance()
        {
            if (_userAuth == null)
            {
                _userAuth = new UserAuth();
            }

            return _userAuth;
        }

        public int UserLogin(LogOnModel model)
        {
            return 1;
        }

        public bool RegisterUser(RegisterModel model)
        {
            
            return true;
        }

        public UserListViewModel GetUserList(string userName)
        {
            var model = new UserListViewModel() { UserId = new int[] { 33, 3 }, UserName = new string[] { "Wesley", "Tom" }, UserCount = 2, SelectedUserIndex = -1 };
            
            return model;
        }
        
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserListViewModel
    {
        public int[] UserId { get; set; }

        public string[] UserName { get; set; }

        public int UserCount { get; set; }

        public int SelectedUserIndex { get; set; }
    }

    public class LogOnModel
    {
        public int UserId { get; set; }
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
