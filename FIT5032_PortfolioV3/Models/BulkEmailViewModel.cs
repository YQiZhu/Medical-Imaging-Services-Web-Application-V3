using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIT5032_PortfolioV3.Models
{
    public class BulkEmailViewModel
    {
        public List<UserRoleViewModel> Patients { get; set; } = new List<UserRoleViewModel>();
        public List<UserRoleViewModel> Staff { get; set; } = new List<UserRoleViewModel>();
        public string EmailSubject { get; set; }
        public string EmailContent { get; set; }
    }

    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool IsSelected { get; set; }
    }

}