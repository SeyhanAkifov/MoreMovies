using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoreMovie.Web.Models;
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
                .ForMember(x => x.Comments, y => y.MapFrom(s => s.Comments.Select(x => x.Comment).ToList()))
                .ForMember(x => x.Actors, y => y.MapFrom(s => s.Actors.Where(z => z.MovieId == s.Id).Select(x => x.Actor.Name).ToList()));

             this.CreateMap<IdentityUser, MyProfileViewModel>()
                .ForMember(x => x.Id, y => y.MapFrom(s => s.Id))
                .ForMember(x => x.Email, y => y.MapFrom(s => s.Email));

            this.CreateMap<News, NewsViewModel>();

            this.CreateMap<ComingSoon, ComingSoonViewModel>();
        }

    }
}
