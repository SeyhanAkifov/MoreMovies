using AutoMapper;
using Moq;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class MapperMock
    {
        public static IMapper Instance
        {
            get
            {
                var mapperMock = new Mock<IMapper>();

                return mapperMock.Object;
            }
        }
    }
}
