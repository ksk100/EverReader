﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EverReader.Models;

namespace EverReader.DataAccess
{
    public interface IEverReaderDataAccess
    {
        IEnumerable<EFDbEvernoteCredentials> EFDbEvernoteCredentials { get; }

        EFDbEvernoteCredentials GetEvernoteCredentials(int? credentialsId);

        void UpdateEvernoteCredentials(EFDbEvernoteCredentials evernoteCredentials);

        Bookmark GetAutomaticBookmark(string userId, string guid);

        void SaveBookmark(Bookmark bookmark);
    }
}
