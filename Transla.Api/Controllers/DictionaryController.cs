using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Api.Contracts;
using Transla.Api.Services;

namespace Transla.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _dictionaryService;
        private readonly ICultureService _cultureService;

        public DictionaryController(IDictionaryService dictionaryService, ICultureService cultureService)
        {
            _dictionaryService = dictionaryService;
            _cultureService = cultureService;
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

        [HttpGet("{service}")]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> Get(string service)
        {
            try
            {
                return Ok(await _dictionaryService.GetAll(service));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{service}/{code}/{cultureName}")]
        public async Task<ActionResult<DictionaryContract>> Get(string service, string cultureName, string code)
        {
            try
            {
                // TODO: check if culture exist
                return Ok(await _dictionaryService.Get(service, code, cultureName));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{service}/{code}")]
        public async Task<ActionResult<IEnumerable<DictionaryContract>>> Get(string service, string code)
        {
            try
            {
                // TODO: check if culture exist
                return Ok(await _dictionaryService.Get(service, code));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{service}/{code}")]
        public async Task<ActionResult> Delete(string service, string code)
        {
            try
            {
                // TODO: check culture
                foreach(var culture in (await _cultureService.GetAll()))
                    await _dictionaryService.Delete(culture.CultureName, service, code);

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
            try
            {
                // TODO: check culture
                foreach(var contract in contracts)
                    await _dictionaryService.Save(contract);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
