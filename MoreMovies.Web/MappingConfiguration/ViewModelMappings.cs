using Microsoft.AspNetCore.Identity;
using MoreMovies.Services.Dto.Input;
using MoreMovies.Services.Dto.Output;
using MoreMovies.Web.Models;
using MoreMovies.Web.Models.Administration;
using MoreMovies.Web.Models.Cinema;
using MoreMovies.Web.Models.Movie;
using MoreMovies.Web.Models.News;

namespace MoreMovies.Web.MappingConfiguration;

internal static class ViewModelMappings
{
    public static MyProfileViewModel ToViewModel(this IdentityUser source) =>
        source is null ? null : new MyProfileViewModel
        {
            Id = source.Id,
            Email = source.Email,
        };

    public static NewsViewModel ToViewModel(this NewsOutputDto source) =>
        source is null ? null : new NewsViewModel
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
        };

    public static ComingSoonViewModel ToViewModel(this ComingSoonOutputDto source) =>
        source is null ? null : new ComingSoonViewModel
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            ImageUrl = source.ImageUrl,
        };

    public static MovieDetailsApiModel ToApiModel(this DetailsOutputDto source) =>
        source is null ? null : new MovieDetailsApiModel
        {
            Description = source.Description,
            Genre = source.Genre,
            Language = source.Language,
            Country = source.Country,
            ReleaseDate = source.ReleaseDate,
            Budget = source.Budget,
            HomePage = source.HomePage,
        };

    public static EditMovieInputModel ToEditModel(this MovieDetailOutputDto source) =>
        source is null ? null : new EditMovieInputModel
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            HomePage = source.HomePage,
            ImageUrl = source.ImageUrl,
        };

    public static MovieViewModel ToViewModel(this MovieOutputDto source) =>
        source is null ? null : new MovieViewModel
        {
            Id = source.Id,
            Title = source.Title,
            Likes = source.Likes,
            HomePage = source.HomePage,
            Rating = source.Rating,
            RatingCount = source.RatingCount,
            ImageUrl = source.ImageUrl,
            CommentsCount = source.CommentsCount,
        };

    public static MovieDetailsViewModel ToViewModel(this MovieDetailOutputDto source) =>
        source is null ? null : new MovieDetailsViewModel
        {
            Id = source.Id,
            Title = source.Title,
            Description = source.Description,
            Likes = source.Likes,
            HomePage = source.HomePage,
            Creator = source.Creator,
            Rating = source.Rating,
            RatingCount = source.RatingCount,
            ImageUrl = source.ImageUrl,
            IsUserLiked = source.IsUserLiked,
        };

    public static CinemaProjectionViewModel ToViewModel(this CinemaProjectionOutputDto source) =>
        source is null ? null : new CinemaProjectionViewModel
        {
            Id = source.Id,
            ProjectionTime = source.ProjectionTime,
            MovieName = source.MovieName,
            Time = source.Time,
            CinemaHall = source.CinemaHall?.Name,
        };
}
