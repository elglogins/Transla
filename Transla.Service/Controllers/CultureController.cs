using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;
using Transla.Service.Interfaces.Services;

namespace Transla.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("TranslaAllowAll")]
    [Authorize(AuthenticationSchemes = AuthenticationSchemas.ManagementAccess)]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _cultureService;
        private readonly IDictionaryService _dictionaryService;
        private readonly IApplicationService _applicationService;

        public CultureController(ICultureService cultureService, IDictionaryService dictionaryService, IApplicationService applicationService)
        {
            _cultureService = cultureService;
            _dictionaryService = dictionaryService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CultureContract>>> Get()
        {
            try
            {
                return Ok(await _cultureService.GetAll());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{cultureName}")]
        public async Task<ActionResult<CultureContract>> Get(string cultureName)
        {
            try
            {
                var result = await _cultureService.Get(cultureName);
                if (result == null)
                    return BadRequest();

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CultureContract contract)
        {
            try
            {
                await _cultureService.Save(contract.CultureName);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{cultureName}")]
        public async Task<ActionResult> Delete(string cultureName)
        {
            try
            {
                await _cultureService.Delete(cultureName);
                // get all applications
                var applications = await _applicationService.GetAll();
                // foreach application delete all dictionaries in this culture
                foreach (var application in applications)
                {
                    var cultureDictionaries = await _dictionaryService.GetAll(application.Alias, cultureName);
                    foreach (var dictionary in cultureDictionaries)
                    {
                        await _dictionaryService.Delete(dictionary.CultureName, dictionary.Application, dictionary.Alias);
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
