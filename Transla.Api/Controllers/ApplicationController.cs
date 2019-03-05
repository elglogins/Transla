using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;
using Transla.Core.Interfaces.Services;

namespace Transla.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IDictionaryService _dictionaryService;

        public ApplicationController(IApplicationService applicationService, IDictionaryService dictionaryService)
        {
            _applicationService = applicationService;
            _dictionaryService = dictionaryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationContract>>> Get()
        {
            try
            {
                return Ok(await _applicationService.GetAll());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{alias}")]
        public async Task<ActionResult<ApplicationContract>> Get(string alias)
        {
            try
            {
                var result = await _applicationService.Get(alias);
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
        public async Task<ActionResult> Post([FromBody] ApplicationContract contract)
        {
            try
            {
                await _applicationService.Save(contract.Alias);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{alias}")]
        public async Task<ActionResult> Delete(string alias)
        {
            try
            {
                await _applicationService.Delete(alias);
                // delete all dictionaries for particular application
                var applicationDictionaries = await _dictionaryService.GetAll(alias);
                foreach(var dictionary in applicationDictionaries)
                {
                    await _dictionaryService.Delete(dictionary.CultureName, dictionary.Application, dictionary.Alias);
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
