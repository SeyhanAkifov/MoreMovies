using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoreMovie.Web.Models.Administration
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            this.Users = new List<string>();
        }
        public string RoleId { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
