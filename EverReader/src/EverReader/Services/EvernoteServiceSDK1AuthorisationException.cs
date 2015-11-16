using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EverReader.Services
{
    public class EvernoteServiceSDK1AuthorisationException : Exception
    {
        public EvernoteServiceSDK1AuthorisationException()
        {
        }

        /*public EvernoteServiceSDK1AuthorisationException(EDAMUserException ex)
        {
            this.InnerException = ex;
        }*/
    }
}
