# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

MoreMovies is an ASP.NET Core MVC app (.NET 10, C#) for browsing movies, news, and upcoming releases, with a cinema/showtimes feature and user comments/ratings. Auth is ASP.NET Core Identity (cookie-based). No third-party movie API is used — all movie/news/coming-soon data is seeded locally from JSON files.

## Commands

```bash
dotnet build MoreMovies.sln
dotnet run --project MoreMovies.Web        # https://localhost:5001, http://localhost:5000
dotnet test MoreMovie.Web.Tests            # xUnit + Moq
```

Note the test project is `MoreMovie.Web.Tests` — **singular "Movie"**, unlike every other project (`MoreMovies.*`). Easy to typo in commands/paths.

### Local setup required before first run

`MoreMovies.Web/appsettings.json` is deleted from the repo and gitignored — it must be created locally with a `ConnectionStrings:DefaultConnection` key (SQL Server) before the app will start. The Web project also has a `UserSecretsId`, so user-secrets is the intended place for local secrets instead of committing them to `appsettings.json`.

On every startup, `Program.cs` calls `app.PrepareDatabaseAsync()` ([MoreMovies.Web/Infrastructure/ApplicationBuilderExtensions.cs](MoreMovies.Web/Infrastructure/ApplicationBuilderExtensions.cs)), which runs `db.Database.MigrateAsync()` and then seeds Genre/Language/Country/Users/Movies/News/ComingSoon data (idempotent via `.Any()` checks). Movies/News/ComingSoon seed content comes from `movies.json`, `news.json`, `comingSoon.json` in the Web project root. Development demo users are created without passwords. Existing accounts keep their current credentials.

## Architecture

### Project/dependency layering

```
MoreMovie.Web.Tests → MoreMovies.Web → MoreMovies.Services → MoreMovies.Data → MoreMovies.Models
```

All projects target `net10.0`. There is no repository/unit-of-work abstraction — `MoreMovies.Services` classes take `ApplicationDbContext` directly via constructor injection alongside other services.

- **MoreMovies.Models** — domain entities (`Actor`, `Movie`, `Genre`, `Language`, `Country`, `Comment`, `News`, `ComingSoon`, `Cinema`, `CinemaHall`, `CinemaPojection`, join entities `MovieActor`/`MovieComment`/`MovieCountry`/`MovieGenre`/`MovieLanguage`/`UserMovie`), plus `DataConstants.cs` (validation constants). `ApplicationUser.cs` is defined here but unused — the app registers `IdentityUser` directly in the DbContext and Identity setup, not `ApplicationUser`.
- **MoreMovies.Data** — `ApplicationDbContext : IdentityDbContext<IdentityUser>` ([MoreMovies.Data/ApplicationDbContext.cs](MoreMovies.Data/ApplicationDbContext.cs)) combines app tables and ASP.NET Identity tables in one context. Composite keys for all many-to-many join entities are configured in `OnModelCreating`. 20 EF Core migrations exist, dated 2021-05-27 through 2021-12-29 — none since the .NET 10 migration, so the schema has been stable since then.
- **MoreMovies.Services** — one service + interface per domain area (`MovieService`/`IMovieService`, `ActorService`, `CinemaService`, `CommentService`, `GenreService`, `LanguageService`, `CountryService`, `NewsService`, `ComingSoonService`, `UserService`), plus `Validator`/`IValidator`. `Dto/Input` and `Dto/Output` hold the request and response DTOs used at the service boundary.
- **MoreMovies.Web** — ASP.NET Core MVC app using the minimal hosting model. Service registrations and the request pipeline are configured in `Program.cs`:
  - `AddDbContext<ApplicationDbContext>` using `Configuration.GetConnectionString("DefaultConnection")`
  - `AddDefaultIdentity<IdentityUser>(RequireConfirmedAccount = true).AddRoles<IdentityRole>()` — cookie auth, roles `Admin`/`User`, no JWT/external OAuth
  - Global `AutoValidateAntiforgeryTokenAttribute` filter on all controllers
  - Typed mapping methods convert service DTOs to Razor and API models
  - `AddSignalR()`, with all domain services registered `AddScoped`
  - `MapHub<MovieHub>("/moviehub")` for live comment notifications
  - Has its own `Models/{Administration,Cinema,Comingsoon,Movie,News}` folder for view/API models — distinct from the domain entities in `MoreMovies.Models`
  - `Controllers/Api/DetailsApiController.cs` is a separate `[ApiController]` JSON endpoint (`api/[action]/{id}`) alongside the regular MVC controllers
  - `Areas/Identity/Pages` only tracks `_ViewStart.cshtml` — the actual Identity UI comes from the `Microsoft.AspNetCore.Identity.UI` package, not customized here

### Known naming quirks (intentional, not typos to "fix" opportunistically)

- `CinemaPojection` (missing "r") is the entity/DbSet name and appears throughout migrations and services; some DTOs use the correct spelling (`CinemaProjectionInputDto`), so the two spellings coexist by design — don't rename one without checking both.
- A stray `wwwroot/css/movies.css` exists at the repo root, outside any project; it appears unused.

### Tests (`MoreMovie.Web.Tests`)

xUnit + Moq, `Microsoft.AspNetCore.Mvc.Testing`, `EntityFrameworkCore.InMemory`. Hand-written mocks live under `Mocks/` (`DatabaseMock.cs`, `MapperMock.cs`, `MemoryCacheMock.cs`, `MovieControllerMock.cs`, `Mocks/Services/{CommentServiceMock,MovieServiceMock}.cs`) rather than a general mocking convention — follow the existing mock style for a given area when adding tests there.
