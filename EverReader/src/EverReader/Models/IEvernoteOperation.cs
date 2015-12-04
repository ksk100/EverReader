using Evernote.EDAM.Type;
using EverReader.Services;

namespace EverReader.Models
{
    interface IEvernoteOperation
    {
        void PerformOperation(IEvernoteService evernoteService, Note note);
    }
}
