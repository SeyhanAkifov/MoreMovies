
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

        public async Task GetUpdateForComment()
        {
            //CheckResult result;
            //do
            //{
            //    result = this.commentService.AddComment();
            //    if (result.New)
            //    {
            //        await this.Clients.Caller.SendAsync("RecieveMessage", result.Update);
            //    }
            //}
            //while (!result.Finished);
            //await this.Clients.Caller.SendAsync("Finished");
        }
    }
}
