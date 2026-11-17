using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using MoreMovies.Services.Dto.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Service;

public class CinemaServiceTests
{
    [Fact]
    public async Task OnlyCinemaOwnerCanChangeItsProjections()
    {
        using var db = DatabaseMock.Instance;
        db.Cinemas.Add(new Cinema { Name = "Test cinema", UserId = "owner" });
        await db.SaveChangesAsync();

        var service = new CinemaService(db, null);
        var projection = new CinemaProjectionInputDto
        {
            CinemaName = "Test cinema",
            CinemaHallName = "Hall 1",
            MovieName = "Test film",
            ProjectionTime = new DateTime(2026, 11, 17),
            Time = "20:00"
        };

        Assert.False(await service.Add(projection, "other-user"));
        Assert.Empty(db.CinemaHalls);
        Assert.True(await service.Add(projection, "owner"));

        var id = db.CinemaPojections.Single().Id;
        Assert.False(await service.Delete(id, "other-user"));
        Assert.Single(db.CinemaPojections);
        Assert.True(await service.Delete(id, "owner"));
        Assert.Empty(db.CinemaPojections);
    }
}
