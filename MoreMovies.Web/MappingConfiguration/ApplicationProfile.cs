﻿using AutoMapper;
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
            this.CreateMap<IdentityUser, MyProfileViewModel>()
               .ForMember(x => x.Id, y => y.MapFrom(s => s.Id))
               .ForMember(x => x.Email, y => y.MapFrom(s => s.Email));

            this.CreateMap<NewsOutputDto, NewsViewModel>();

            this.CreateMap<ComingSoonOutputDto, ComingSoonViewModel>();
            
            this.CreateMap<DetailsOutputDto, MovieDetailsApiModel>();
            
            this.CreateMap<DetailsOutputDto, MovieDetailsViewModel>();
            
            this.CreateMap<MovieOutputDto, EditMovieInputModel>();

            this.CreateMap<MovieOutputDto, MovieViewModel>();
            
            this.CreateMap<MovieDetailOutputDto, MovieDetailsViewModel>();

        }
    }
}
