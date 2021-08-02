using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoreMovie.Web.Models.Administration;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Movie;
using MoreMovies.Web.Models.News;
using System.Linq;

namespace SocialNetworkCustom.Web.MappingConfiguration
{
    public class ApplicationProfile : Profile
    {

        public ApplicationProfile()
        {
            //this.CreateMap<Movie, MovieViewModel>()
            //    .ForMember(x => x.Comments, y => y.MapFrom(s => s.Comments.Select(x => x.Comment).ToList()))
            //    .ForMember(x => x.Actors, y => y.MapFrom(s => s.Actors.Where(z => z.MovieId == s.Id).Select(x => x.Actor.Name).ToList()));

            this.CreateMap<IdentityUser, MyProfileViewModel>()
               .ForMember(x => x.Id, y => y.MapFrom(s => s.Id))
               .ForMember(x => x.Email, y => y.MapFrom(s => s.Email));

            this.CreateMap<NewsOutputDto, NewsViewModel>();

            this.CreateMap<ComingSoonOutputDto, ComingSoonViewModel>();

            this.CreateMap<DetailsOutputDto, MovieDetailsViewModel>();

            this.CreateMap<MovieOutputDto, EditMovieInputModel>();

            this.CreateMap<MovieOutputDto, MovieViewModel>()
                .ForMember(x => x.Comments, y => y.MapFrom(s => s.Comments.Select(x => x.Comment).ToList()))
                .ForMember(x => x.Actors, y => y.MapFrom(s => s.Actors.Where(z => z.MovieId == s.Id).Select(x => x.Actor.Name).ToList()));

        }

    }
}
