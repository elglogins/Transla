using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transla.Contracts;
using Transla.Service.Interfaces.Services;

namespace Transla.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TranslaAllowAll")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemas.ManagementAccess)]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly ICultureService _cultureService;
        private readonly IApplicationService _applicationService;

        public DictionaryController(IDictionaryService dictionaryService, ICultureService cultureService, IApplicationService applicationService)
        {
            _dictionaryService = dictionaryService;
            _cultureService = cultureService;
            _applicationService = applicationService;
        }

        [HttpGet("/application-grouped")]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> GetApplicationGrouped()
        {
            try
            {
                var results = new List<ApplicationGroupedDictionariesContract>();
                var allDictionaries = await _dictionaryService.GetAll();
                var applicationGrouped = allDictionaries.GroupBy(g => g.Application);
                foreach (var applicationGroup in applicationGrouped)
                {
                    var dictionaries = new List<ApplicationGroupedDictionariesContract.GroupedCultureDictionariesContract>();
                    var result = new ApplicationGroupedDictionariesContract()
                    {
                        Alias = applicationGroup.Key
                    };

                    // process dictionaries and group by culture
                    var applicationDictionariesGroupedByAlias = applicationGroup.GroupBy(g => g.Alias);
                    foreach (var serviceDictionaryGroup in applicationDictionariesGroupedByAlias)
                    {
                        dictionaries.Add(new ApplicationGroupedDictionariesContract.GroupedCultureDictionariesContract()
                        {
                            Alias = serviceDictionaryGroup.Key,
                            Dictionaries = serviceDictionaryGroup
                        });
                    }

                    result.Dictionaries = dictionaries.OrderBy(o => o.Alias);
                    results.Add(result);
                }

                return Ok(results);
            }
            catch (Exception e)
            {
                return BadRequest(JsonConvert.SerializeObject(e));
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> Get()
        {
            try
            {
                return Ok(await _dictionaryService.GetAll());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{applicationAlias}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> Get(string applicationAlias)
        {
            try
            {
                return Ok(await _dictionaryService.GetAll(applicationAlias));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{applicationAlias}/{code}/{cultureName}")]
        [AllowAnonymous]
        public async Task<ActionResult<DictionaryContract>> Get(string applicationAlias, string cultureName, string code)
        {
            try
            {
                return Ok(await _dictionaryService.Get(applicationAlias, code, cultureName));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{applicationAlias}/{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> Get(string applicationAlias, string code)
        {
            try
            {
                return Ok(await _dictionaryService.Get(applicationAlias, code));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{applicationAlias}/{code}")]
        public async Task<ActionResult> Delete(string applicationAlias, string code)
        {
            try
            {
                await ValidateApplications(new string[] { applicationAlias });

                foreach (var culture in (await _cultureService.GetAll()))
                    await _dictionaryService.Delete(culture.CultureName, applicationAlias, code);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] List<DictionaryContract> contracts)
        {
            if (contracts == null || !contracts.Any())
                return BadRequest("Invalid data");

            try
            {
                await ValidateCultures(contracts.Select(s => s.CultureName).ToArray());
                await ValidateApplications(contracts.Select(s => s.Application).ToArray());

                foreach (var contract in contracts)
                    await _dictionaryService.Save(contract);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        #region Privates

        private async Task ValidateApplications(string[] aliases)
        {
            var availableApplications = await _applicationService.GetAll();
            foreach (var application in aliases)
            {
                if (!availableApplications.Any(a => a.Alias == application))
                    new Exception($"Invalid application used '{application}'");
            }
        }

        private async Task ValidateCultures(string[] cultures)
        {
            var availableCultures = await _cultureService.GetAll();
            foreach (var culture in cultures)
            {
                if (!availableCultures.Any(a => a.CultureName == culture))
                    new Exception($"Invalid culture used '{culture}'");
            }
        }

        #endregion
    }
}
