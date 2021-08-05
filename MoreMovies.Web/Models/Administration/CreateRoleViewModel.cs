using System.ComponentModel.DataAnnotations;

namespace MoreMovies.Web.Models.Administration
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
