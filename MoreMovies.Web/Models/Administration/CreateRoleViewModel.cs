using System.ComponentModel.DataAnnotations;

namespace MoreMovie.Web.Models.Administration
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
