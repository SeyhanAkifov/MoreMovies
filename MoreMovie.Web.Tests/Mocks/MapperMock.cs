using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovie.Web.Tests.Mocks
{
    public static class MapperMock
    {
        public static IMapper Instanse
        {
            get
            {
                var mapperMock = new Mock<IMapper>();

                return mapperMock.Object;
            }
        }
    }
}
