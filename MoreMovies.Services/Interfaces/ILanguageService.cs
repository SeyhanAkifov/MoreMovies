﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMovies.Services.Interfaces
{
    public interface ILanguageService
    {
        public Task Add(string name);
    }
}
