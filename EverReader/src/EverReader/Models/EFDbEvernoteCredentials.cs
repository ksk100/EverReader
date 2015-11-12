using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AsyncOAuth.Evernote.Simple;

namespace EverReader.Models
{
    public class EFDbEvernoteCredentials : EvernoteCredentials
    {
        /// <summary>
        /// Added for EF framework.
        /// </summary>
        public int Id { get; set; }

        public EFDbEvernoteCredentials()
        {
        }

        public EFDbEvernoteCredentials(int id, string authToken, DateTime expires, string notebookUrl, string shard, string userId, string webApiUrlPrefix)
        {
            this.Id = id;
            this.AuthToken = authToken;
            this.Expires = expires;
            this.NotebookUrl = notebookUrl;
            this.Shard = shard;
            this.UserId = userId;
            this.WebApiUrlPrefix = webApiUrlPrefix;
        }

        public EFDbEvernoteCredentials(EvernoteCredentials evernoteCredentials)
        {
            this.AuthToken = evernoteCredentials.AuthToken;
            this.Expires = evernoteCredentials.Expires;
            this.NotebookUrl = evernoteCredentials.NotebookUrl;
            this.Shard = evernoteCredentials.Shard;
            this.UserId = evernoteCredentials.UserId;
            this.WebApiUrlPrefix = evernoteCredentials.WebApiUrlPrefix;
        }
    }
}
