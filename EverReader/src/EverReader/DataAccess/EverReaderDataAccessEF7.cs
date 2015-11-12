using EverReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace EverReader.DataAccess
{
    public class EverReaderDataAccessEF7 : IEverReaderDataAccess
    {
        private readonly EverReaderContext _dbContext;

        public IEnumerable<EFDbEvernoteCredentials> EFDbEvernoteCredentials
        {
            get
            {
                return _dbContext.EvernoteCredentials;
            }
        }

        public EverReaderDataAccessEF7(EverReaderContext dbContext)
        {
            _dbContext = dbContext;
        }

        public EFDbEvernoteCredentials GetEvernoteCredentials(int? credentialsId)
        {
            if (credentialsId == null) throw new ArgumentNullException();
            return _dbContext.EvernoteCredentials.SingleOrDefault(a => a.Id == credentialsId);
        }

        public void UpdateEvernoteCredentials(EFDbEvernoteCredentials evernoteCredentials)
        {
            _dbContext.EvernoteCredentials.Add(evernoteCredentials);
            _dbContext.SaveChanges();
        }

    }
}
