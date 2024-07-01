using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Transcription;
using Transdit.Utilities.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using Transdit.API.Configuration.Attributes;
using Google;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [ServiceFilter(typeof(PreTranscriptionValidatationAttribute))]
    [Authorize]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TranscribeController : ControllerBase
    {
        private readonly ITranscriptionsService _transcriber;
        private readonly IGoogleBucket _googleBucket;
        private readonly IGoogleSpeech _googleSpeech;
        private readonly ILogger<TranscribeController> _logger;
        private readonly IUsersService _usersService;
        private readonly ITranscriptionMediaConvertion _mediaConverter;
        private readonly AppConfiguration _configurations;
        private readonly IYoutubeDownloader _youtubeDownloader;
        private IEnumerable<string> _expectedGoogleExtensions;

        public TranscribeController(ILogger<TranscribeController> logger, IUsersService usersService, ITranscriptionMediaConvertion mediaConverter,
            AppConfiguration configurations, IGoogleBucket googleBucket, ITranscriptionsService transcriber, IGoogleSpeech googleSpeech, IYoutubeDownloader youtubeDownloader)
        {
            _googleBucket = googleBucket;
            _logger = logger;
            _usersService = usersService;
            _mediaConverter = mediaConverter;
            _configurations = configurations;
            _transcriber = transcriber;
            _googleSpeech = googleSpeech;
            _youtubeDownloader = youtubeDownloader;
            _expectedGoogleExtensions = _googleSpeech.GetExpectedAudioEncodings().Keys.ToArray();
        }

        [HttpPost]
        [Route("youtube")]
        public async Task<IActionResult> TranscribeYoutube([FromBody] InputTranscription transcription, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!transcription.IsYoutubeMidia)
                    return BadRequest("Não foi enviado um link válido do Youtube");

                var user = await GetCurrentUser();
                transcription.PhysicalPath = Path.Combine(_mediaConverter.TempFolderPath, Path.GetRandomFileName());
                transcription.FileName = await _youtubeDownloader.DownloadAudioAsync(transcription.YoutubeUrl, transcription.PhysicalPath, true, cancellationToken);

                var converted = await _mediaConverter.Convert(transcription);
                if (converted)
                {
                    _mediaConverter.GetMetadata(transcription, transcription.ConvertedPath);
                    System.IO.File.Delete(transcription.PhysicalPath);
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Houve um erro interno ao processar vídeo do Youtube.");

                var uploadResult = await _googleBucket.Upload(transcription.ConvertedPath, cancellationToken);
                if (uploadResult is not null)
                {
                    transcription.StorageUri = _googleBucket.GetUri(uploadResult);
                    transcription.StorageFileName = uploadResult.Name;
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Houve um erro interno ao salvar seu arquivo.");

                var transcriptionYoutubeResult = await _transcriber.Transcribe(transcription, user, cancellationToken);
                transcriptionYoutubeResult.StorageUri = uploadResult.MediaLink;

                return Ok(transcriptionYoutubeResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro interno ao transcrever a mídia do youtube: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{transcriptionId}")]
        public async Task<IActionResult> Transcribe([FromRoute] int transcriptionId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (transcriptionId <= 0)
                    return BadRequest("Identificador para transcrição inválido.");

                var user = await GetCurrentUser();
                var transcription = _transcriber.Get(transcriptionId, user);
                if (transcription is null)
                    return BadRequest("Transcrição não localizada para o usuário.");

                if ((transcription.Date - DateTime.Now).Days > 3)
                    return BadRequest("Transcrições feitas há mais de três dias não podem ser reutilizadas.");

                var storageFile = await _googleBucket.Get(transcription.StorageFileName);
                if (storageFile is null)
                    return BadRequest("O arquivo não está mais disponível no armazenamento. Será necessário refazer a transcrição.");

                var storageFileUri = _googleBucket.GetUri(storageFile);
                var transcriptionResult = await _googleSpeech.Transcribe(storageFileUri, transcription.Language, cancellationToken);
                var transcriptionList = _googleSpeech.SpeechResponseToTranscriptionList(transcriptionResult);

                var result = new TranscriptionOperationResult() { Successful = true, Data = transcriptionList, FileName = transcription.StorageFileName, StorageUri = storageFile.MediaLink};

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }

        }

        [HttpGet]
        [Route("storage/{storageFileName}")]
        public async Task<IActionResult> Transcribe([FromRoute] string storageFileName, [FromQuery] string lang, CancellationToken cancellationToken = default)
        {
            try
            {
                var user = await GetCurrentUser();
                var storageFile = await _googleBucket.Get(storageFileName);
                if (storageFile is null)
                    return BadRequest("O arquivo não está mais disponível no armazenamento. Será necessário refazer a transcrição.");

                var storageFileUri = _googleBucket.GetUri(storageFile);
                var transcriptionResult = await _googleSpeech.Transcribe(storageFileUri, lang, cancellationToken); 
                var transcriptionList = _googleSpeech.SpeechResponseToTranscriptionList(transcriptionResult);

                var result = new TranscriptionOperationResult() { Successful = true, Data = transcriptionList, FileName = storageFileName, StorageUri = storageFile.MediaLink };
                result.Messages.Add("Transcrição concluída com sucesso.");

                return Ok(result);
            }
            catch( GoogleApiException gEx)
            {
                if(gEx.HttpStatusCode == HttpStatusCode.NotFound)
                    return BadRequest("O arquivo não está mais disponível no armazenamento. Será necessário refazer a transcrição.");
                return BadRequest(gEx.Error.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Transcribe([FromBody] InputTranscription transcription, CancellationToken cancellationToken = default)
        {
            try
            {
                if (transcription is null)
                    return BadRequest("Formulário não tem informações sobre transcrição.");

                var user = await GetCurrentUser();
                var filePath = transcription.IsConverted 
                    ? Path.Combine(_mediaConverter.TempConvertedFolderPath, transcription.FileName) 
                    : Path.Combine(_mediaConverter.TempFolderPath, transcription.FileName);
                if (!System.IO.File.Exists(filePath))
                    return BadRequest("Houve um problema ao processar seu arquivo.");

                _mediaConverter.GetMetadata(transcription, filePath);

                var uploadResult = await _googleBucket.Upload(filePath, cancellationToken);
                if (uploadResult is not null)
                {
                    transcription.StorageUri = _googleBucket.GetUri(uploadResult);
                    transcription.StorageFileName = uploadResult.Name;
                }
                else
                    return StatusCode(StatusCodes.Status500InternalServerError, "Houve um erro interno ao salvar seu arquivo.");

                var transcriptionResult = await _transcriber.Transcribe(transcription, user, cancellationToken);
                transcriptionResult.StorageUri = uploadResult.MediaLink;

                return Ok(transcriptionResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar transcrição", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
            }
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                    return BadRequest($"Não foi possível concluir a operação, conteúdo corrompido.");

                var user = await GetCurrentUser();
                string trustedFileNameForFileStorage = string.Empty;
                string trustedFileNameForDisplay = string.Empty;
                string trustedConvertedFileNameForDisplay = string.Empty;
                string outputFilePath = string.Empty;

                var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType),  (int)_configurations.FileSizeLimit);
                var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                var section = await reader.ReadNextSectionAsync();

                while (section != null)
                {
                    var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);
                    if (hasContentDispositionHeader)
                    {
                        if (!MultipartRequestHelper
                            .HasFileContentDisposition(contentDisposition))
                        {
                            ModelState.AddModelError("File", $"Houve um erro ao processar sua solicitação, tente novamente em instantes.");
                            return BadRequest(ModelState);
                        }
                        else
                        {
                            trustedFileNameForDisplay = WebUtility.HtmlEncode(contentDisposition.FileName.Value);
                            trustedFileNameForFileStorage = Path.GetRandomFileName();
                            outputFilePath = Path.Combine(_mediaConverter.TempFolderPath, trustedFileNameForFileStorage);
                            var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition, ModelState, _configurations.PermitedFileTypes.ToArray(), _configurations.FileSizeLimit);

                            if (!ModelState.IsValid)
                                return BadRequest(ModelState);

                            using (var targetStream = System.IO.File.Create(outputFilePath))
                            {
                                await targetStream.WriteAsync(streamedFileContent);
                                _logger.LogInformation($"Salvando arquivo temporário {trustedFileNameForDisplay} como {trustedFileNameForFileStorage} qual foi criado por {user.UserName}.");
                            }
                        }
                    }
                    section = await reader.ReadNextSectionAsync();
                }
                bool wasConverted = false;
                var fileExtension = Path.GetExtension(trustedFileNameForDisplay);

                if (!_expectedGoogleExtensions.Contains(fileExtension))
                {
                    var converted = await _mediaConverter.Convert(outputFilePath);
                    if (converted.Item1)
                    {
                        System.IO.File.Delete(outputFilePath);

                        trustedFileNameForFileStorage = string.Empty;
                        trustedConvertedFileNameForDisplay = Path.GetFileName(converted.Item2);
                        wasConverted = converted.Item1;
                    }
                }

                return Ok(new { fileName = trustedFileNameForFileStorage, convertedFileName = trustedConvertedFileNameForDisplay, isConverted = wasConverted });
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
