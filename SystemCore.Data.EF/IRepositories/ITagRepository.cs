﻿using System;
using System.Collections.Generic;
using System.Text;
using SystemCore.Data.Entities;
using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF.IRepositories
{
    public interface ITagRepository : IRepository<Tag, string>
    {
    }
}
