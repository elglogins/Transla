using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Transla.Api.Contracts;
using Transla.Api.Services;

namespace Transla.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CultureController : ControllerBase
    {
        private readonly ICultureService _cultureService;

        public CultureController(ICultureService cultureService)
        {
            _cultureService = cultureService;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CultureContract>>> Get()
        {
            try
            {
                return Ok(await _cultureService.GetAll());
            }
            catch(Exception)
            {
                return BadRequest();
            }
        }

        // GET api/values/5
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

        // POST api/values
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

        // DELETE api/values/5
        [HttpDelete("{cultureName}")]
        public async Task<ActionResult> Delete(string cultureName)
        {
            try
            {
                await _cultureService.Delete(cultureName);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
