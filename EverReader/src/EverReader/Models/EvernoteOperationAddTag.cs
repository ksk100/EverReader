using Evernote.EDAM.Type;
using EverReader.Services;

namespace EverReader.Models
{
    public class EvernoteOperationAddTag : IEvernoteOperation
    {
        private string _tagGuid;

        public EvernoteOperationAddTag(string tagGuid)
        {
            _tagGuid = tagGuid;
        }

        public void PerformOperation(IEvernoteService evernoteService, Note note)
        {
        }
    }
}
