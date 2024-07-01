using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;

namespace Transdit.Core.Contracts
{
    public interface ITranscriptionsService
    {
        void Add(Transcription transcription);
        void Update(Transcription transcription);
        void Delete(int id);
        IEnumerable<Transcription> Get(ApplicationUser user);
        Transcription? Get(int id, ApplicationUser user);
        PaginatedList<Transcription> Get(PaginationInput pagination, ApplicationUser user, string route);
        IEnumerable<string> GetEncodingTypes();
        Dictionary<string, IEnumerable<string>> GetLanguageCodes();
        TimeSpan TotalMonthlyUsage(ApplicationUser user, DateTime date);
        Task<TranscriptionOperationResult> Transcribe(InputTranscription transcription, ApplicationUser user, CancellationToken cancellationToken = default);
    }
}
