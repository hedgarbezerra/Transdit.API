using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Transcription;
using Transdit.Services.Transcriptions;

namespace Transdit.API.Controllers.V1
{
    [ExcludeFromCodeCoverage]
    [Authorize]
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/dictionaries")]
    [Route("api/v{version:apiVersion}/dictionaries")]
    [ApiController]
    public class CustomDictionaryController : ControllerBase
    {
        private readonly ICustomDictionariesService _dictionariesService;
        private readonly ILogger<CustomDictionaryController> _logger;
        private readonly IUsersService _usersService;
        private readonly IMapper _mapper;

        public CustomDictionaryController(ICustomDictionariesService dictionariesService, ILogger<CustomDictionaryController> logger, IUsersService usersService, IMapper mapper)
        {
            _dictionariesService = dictionariesService;
            _logger = logger;
            _usersService = usersService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("paginate")]
        public async Task<IActionResult> Paginate([FromQuery] PaginationInput pagination)
        {
            try
            {
                var user = await GetCurrentUser();
                string route = HttpContext.Request.Path.Value;
                var paginatedResult = _dictionariesService.GetDictionaries(pagination, route, user);
                var mappedPaginatedResult = _mapper.Map<PaginatedList<OutCustomDictionary>>(paginatedResult);
                return Ok(mappedPaginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var user = await GetCurrentUser();
                var paginatedResult = _dictionariesService.GetDictionaries(user);
                if (paginatedResult.Count() <= 0)
                    return NoContent();

                var mappedPaginatedResult = _mapper.Map<IEnumerable<OutCustomDictionary>>(paginatedResult);
                return Ok(mappedPaginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }

        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                var dict = _dictionariesService.GetDictionary(id);
                if (dict is null)
                    return NoContent();

                var mappedDict = _mapper.Map<OutCustomDictionary>(dict);
                return Ok(mappedDict);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InCustomDictionary dictionary)
        {
            try
            {
                var user = await GetCurrentUser();
                if (user is null)
                    return BadRequest($"Houve um problema de autenticação com sua solicitação.");

                var mappedDict = _mapper.Map<CustomDictionary>(dictionary);
                mappedDict.UserId = user.Id;

                _dictionariesService.NewDictionary(mappedDict);
                return Ok(mappedDict.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody] OutCustomDictionary updateDictionary)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(updateDictionary.Id);
                if (dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {updateDictionary.Id} localizado.");

                dictionary.Name = updateDictionary.Name;
                dictionary.Description = updateDictionary.Description;
                _dictionariesService.UpdateDictionary(dictionary);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute]int id)
        {
            try
            {
                var user = await GetCurrentUser();
                _dictionariesService.DeleteDictionary(id, user);

                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("{idDictionary}/words")]
        public IActionResult PostWord([FromRoute]int idDictionary, [FromBody] OutCustomDictionaryWord word)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(idDictionary);
                if (dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {idDictionary} localizado.");

                _dictionariesService.AddWord(word.Word, idDictionary);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }
        [HttpPost]
        [Route("{idDictionary}/words/multiple")]
        public IActionResult PostWord([FromRoute]int idDictionary, [FromBody] IEnumerable<OutCustomDictionaryWord> words)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(idDictionary);
                if (dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {idDictionary} localizado.");
                foreach (var word in words)
                    _dictionariesService.AddWord(word.Word, idDictionary);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{idDictionary}/words")]
        public IActionResult GetWords(int idDictionary)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(idDictionary);
                if(dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {idDictionary} localizado.");

                var words = dictionary.Words;
                if (words.Count <= 0)
                    return NoContent();

                var mappedWords = _mapper.Map<ICollection<OutCustomDictionaryWord>>(words);
                return Ok(mappedWords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("{idDictionary}/words/{idWord}")]
        public IActionResult GetWord(int idDictionary, int idWord)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(idDictionary);
                if (dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {idDictionary} localizado.");

                var word = dictionary.Words.FirstOrDefault(w => w.Id == idWord);
                if(word is null)
                    return NoContent();

                var mappedWord = _mapper.Map<OutCustomDictionaryWord>(word);
                return Ok(mappedWord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{idDictionary}/words/{idWord}")]
        public IActionResult DeleteWord(int idDictionary, int idWord)
        {
            try
            {
                var dictionary = _dictionariesService.GetDictionary(idDictionary);
                if (dictionary is null)
                    return BadRequest($"Nenhum dicionário customizado com identificador {idDictionary} localizado.");

                var word = dictionary.Words.FirstOrDefault(w => w.Id == idWord);
                if (word is null)
                    return BadRequest($"Nenhuma palavra com identificador {idWord} localizada.");

                _dictionariesService.DeleteWord(idWord, dictionary);
                return StatusCode(StatusCodes.Status202Accepted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ocorreu um erro ao processar sua solicitação: {ex.Message}");
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
