
using Microsoft.AspNetCore.SignalR;
using MoreMovies.Services.Interfaces;
using System.Threading.Tasks;



namespace MoreMovies.Web.Hubs
{
    public class MovieHub : Hub
    {
        private readonly ICommentService commentService;

        public MovieHub(ICommentService commentService)
        {
            this.commentService = commentService;
        }

       
    }
}
