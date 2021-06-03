using AutoMapper;
using MoreMovies.Models;
using MoreMovies.Web.Models;
using System.Linq;

namespace SocialNetworkCustom.Web.MappingConfiguration
{
    public class ApplicationProfile : Profile
    {

        public ApplicationProfile()
        {
            this.CreateMap<Movie, MovieViewModel>()
                .ForMember(x => x.Genre, y => y.MapFrom(s => s.Genre.Genre.Name))
                .ForMember(x => x.Language, y => y.MapFrom(s => s.Language.Language.Name))
                .ForMember(x => x.Country, y => y.MapFrom(s => s.Country.Country.Name))
                .ForMember(x => x.Comments, y => y.MapFrom(s => s.Comments.Select(x => x.Comment.Description).ToList()));

            //this.CreateMap<Comment, CommentViewModel>()
            //    .ForMember(x => x.Author, y => y.MapFrom(s => s.User.FirstName + " " + s.User.LastName));


            //this.CreateMap<Post, PostAllViewModel>()
            //    .ForMember(x => x.Author, y => y.MapFrom(s => s.ApplicationUser.FirstName + " " + s.ApplicationUser.LastName));


            //this.CreateMap<ApplicationUser, RegisterInputModel>();





            //this.CreateMap<ApplicationUser, MyProfileViewModel>()
            //    .ForMember(x => x.CityName, y => y.MapFrom(s => s.Address.City.Name))
            //    .ForMember(x => x.Street, y => y.MapFrom(s => s.Address.Street))
            //    .ForMember(x => x.StreetNumber, y => y.MapFrom(s => s.Address.Number));









        }

    }
}
