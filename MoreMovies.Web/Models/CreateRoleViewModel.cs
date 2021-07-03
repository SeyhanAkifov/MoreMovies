using System.ComponentModel.DataAnnotations;

namespace MoreMovie.Web.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
