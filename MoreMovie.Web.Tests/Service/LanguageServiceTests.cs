using MoreMovie.Web.Tests.Mocks;
using MoreMovies.Models;
using MoreMovies.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovie.Web.Tests.Service
{
    public class LanguageServiceTests
    {
        [Fact]
        public static async void GetAllLanguageShouldreturnCorrectCount()
        {
            var data = DatabaseMock.Instance;

            data.Languages.Add(new Language
            {
               Name = "English"
            });

            data.SaveChanges();

            var languageService = new LanguageService(data);

            //Act
            var result = await languageService.GetLanguages();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            

        }

        [Fact]
        public static async void AddLanguageShouldAddCorectly()
        {
            var data = DatabaseMock.Instance;

            var name = "English";
            

            

            var languageService = new LanguageService(data);

            //Act
            await languageService.Add(name);

            //Assert
            
            Assert.Equal(1, data.Languages.Count());
            Assert.Equal(1, data.Languages.First().Id);
            Assert.Equal("English", data.Languages.First().Name);


        }
    }
}
