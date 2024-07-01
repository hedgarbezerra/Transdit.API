using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using NPOI.OpenXml4Net.OPC.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Enums;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Core.Models.Transcriptions;
using Transdit.Services.Common.Convertion;
using Transdit.Services.Transcriptions;
using Transdit.Utilities.Extensions;
using Transdit.Utilities.Helpers;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TranscriptionsController : ControllerBase
    {
        private readonly ILogger<TranscriptionsController> _logger;
        private readonly IFileConverter _fileConverter;
        private readonly IUsersService _usersService;
        private readonly ITranscriptionsService _transcriptionsService;
        private readonly IMapper _mapper;
        private readonly AppConfiguration _configurations;
        public ApplicationUser CurrentUser { get => GetCurrentUser().Result; }
        public TranscriptionsController(IFileConverter fileConverter, IUsersService usersService, ILogger<TranscriptionsController> logger, ITranscriptionsService transcriptionsService, IMapper mapper, AppConfiguration configurations)
        {
            _fileConverter = fileConverter;
            _usersService = usersService;
            _logger = logger;
            _transcriptionsService = transcriptionsService;
            _mapper = mapper;
            _configurations = configurations;
        }

        [HttpPost]
        //[Authorize(Roles ="ADMINISTRATOR")]
        public async Task<IActionResult> Add([FromBody] InputTranscription transcription)
        {
            try
            {
                var mappedTranscription = _mapper.Map<Transcription>(transcription);
                mappedTranscription.UserId = CurrentUser.Id;
                _transcriptionsService.Add(mappedTranscription);

                var output = _mapper.Map<OutTranscription>(mappedTranscription);

                return Created(nameof(TranscriptionsController), output);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Transcription transcription)
        {
            try
            {
                if(_transcriptionsService.Get(transcription.Id, CurrentUser) is null)
                    return NoContent();
                
                transcription.UserId = CurrentUser.Id;
                _transcriptionsService.Update(transcription);
                var output = _mapper.Map<OutTranscription>(transcription);

                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (_transcriptionsService.Get(id, CurrentUser) is null)
                    return NoContent();

                _transcriptionsService.Delete(id);
                return StatusCode((int)HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Paginate([FromQuery] PaginationInput pagination)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return NoContent();

                var route = HttpContext.Request.Path.Value;
                var transcriptions = _transcriptionsService.Get(pagination, user, route);
                var mappedTranscriptions = _mapper.Map<PaginatedList<OutTranscription>>(transcriptions);
                return Ok(mappedTranscriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return NoContent();

                var transcription = _transcriptionsService.Get(id, user);
                if (transcription is null)
                    return NoContent();

                var mappedTranscription = _mapper.Map<OutTranscription>(transcription);
                return Ok(mappedTranscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("export/{transcriptionId}/{format}")]
        public IActionResult Export([FromRoute] int transcriptionId, int format)
        {
            try
            {
                Transcription transcription = _transcriptionsService.Get(transcriptionId, CurrentUser);
                if (transcription is null)
                    return NoContent();

                var outputFormat = (EFileConvertionTarget)format;
                var result = _fileConverter.Convert(transcription.Result, outputFormat);

                return Ok(result.ToArray());

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("export/{format}")]
        public IActionResult Export([FromRoute] int format, [FromBody] TranscriptionOperationResult transcription)
        {
            try
            {
                var outputFormat = (EFileConvertionTarget)format;
                var result = _fileConverter.Convert(transcription, outputFormat, true);

                return Ok(result.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("export")]
        public IActionResult Export([FromBody] ExportInput export)
        {
            try
            {
                var result = _fileConverter.Convert(export.Content, export.Format, true);

                return Ok(result.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro processando o arquivo: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("export/formats")]
        public IActionResult GetFormats()
        {
            var formats = Enum.GetValues<EFileConvertionTarget>().Select(f => f.DisplayName());
            return Ok(formats);
        }

        [HttpGet]
        [Route("languages")]
        public IActionResult GetLanguageCodes()
        {
            try
            {
                var languages = _transcriptionsService.GetLanguageCodes();
                return Ok(languages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao retornar as informações solicitadas: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("permited-files")]
        public IActionResult GetPermittedFiles()
        {
            try
            {
                var typesPermitted = _configurations.PermitedFileTypes;
                return Ok(typesPermitted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }
        }
        private async Task<ApplicationUser> GetCurrentUser()
        {
            var currentUserIdentity = User?.Identity?.Name ?? User?.Claims?.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email))?.Value;
            var user = await _usersService.Get(currentUserIdentity);
            return user;
        }
    }
}
