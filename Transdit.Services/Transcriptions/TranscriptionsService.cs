using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Repository.Repositories;
using Transdit.Services.Common.Convertion;
using Transdit.Utilities.Extensions;

namespace Transdit.Services.Transcriptions
{
    [ExcludeFromCodeCoverage]
    public class TranscriptionsService : ITranscriptionsService
    {
        private readonly IGoogleSpeech _googleSpeech;
        private readonly IUriService _uriService;
        private readonly IPlansService _plansService;
        private readonly IRepository<Transcription> _repository;
        private readonly IMapper _mapper;

        public TranscriptionsService(IGoogleSpeech googleSpeech, IRepository<Transcription> repository, IUriService uriService, IPlansService plansService, IMapper mapper)
        {
            _googleSpeech = googleSpeech;
            _repository = repository;
            _uriService = uriService;
            _plansService = plansService;
            _mapper = mapper;
        }
        public void Add(Transcription transcription)
        {
            _repository.Add(transcription);
            _repository.SaveChanges();
        }
        public void Update(Transcription transcription)
        {
            _repository.Update(transcription);
            _repository.SaveChanges();
        }
        public void Delete(int id)
        {    
            _repository.Delete(id);
            _repository.SaveChanges();
        }
        public Transcription? Get(int id, ApplicationUser user) => _repository.Get(q => q.UserId == user.Id).FirstOrDefault(t => t.Id == id);
        public IEnumerable<Transcription> Get(ApplicationUser user) => _repository.Get(q => q.UserId == user.Id).ToList();
        public PaginatedList<Transcription> Get(PaginationInput pagination, ApplicationUser user, string route)
        {
            pagination.Index += 1;
            var query = _repository.Get(q => q.UserId == user.Id)
                .OrderByDescending(q => q.Date)
                .AsQueryable();
            if (!string.IsNullOrEmpty(pagination.SearchTerm))
            {
                var actualTerm = pagination.SearchTerm.Split(' ');
                for (int i = 0; i < actualTerm.Length; i++)
                {
                    var term = actualTerm[i];
                    query = query.Where(t => t.Name.Contains(term));
                }
            }

            return new PaginatedList<Transcription>(query, _uriService, route, pagination.Index, pagination.Size);
        }
        public async Task<TranscriptionOperationResult> Transcribe(InputTranscription transcription, ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var result = new TranscriptionOperationResult();
            if (CheckMinutesLimit(transcription.LengthInSeconds))
            {
                result.Messages.Add("Limite de 480 minutos por arquivo superado, não será feita transcrição.");
                return result;
            }
            var transcriptionResult = await _googleSpeech.Transcribe(transcription, cancellationToken, transcription.IsLongRunning);
            if(transcriptionResult is null) 
            {
                result.Messages.Add("Houve um erro durante a transcrição do seu arquivo.");
                return result;
            }
            result.Data = _googleSpeech.SpeechResponseToTranscriptionList(transcriptionResult);

            var userPlan = await _plansService.Get(user);
            var baseTranscription = _mapper.Map<Transcription>(transcription);
            baseTranscription.UserId = user.Id;

            if (transcription.Save && userPlan.AllowTranscriptionSaving)
                baseTranscription.Result = _googleSpeech.SpeechResponseToString(transcriptionResult, false);

            Add(baseTranscription);
            return result;
        }

        public TimeSpan TotalMonthlyUsage(ApplicationUser user, DateTime date)
        {
            date.FirstNLastDaysOfMonth(out DateTime min, out DateTime max);
            var transcriptions = Get(user).Where(t => t.Date >= min && t.Date <= max).ToList();

            var total = transcriptions.Sum(t => t.Usage.Ticks);
            return TimeSpan.FromTicks(total);
        }
        public IEnumerable<string> GetEncodingTypes() => _googleSpeech.GetEncodingTypes();
        public Dictionary<string, IEnumerable<string>> GetLanguageCodes() => _googleSpeech.GetLanguageCodes();

        private bool CheckMinutesLimit(double lengthInSeconds) => (lengthInSeconds / 60) >= 480;
    }
}
