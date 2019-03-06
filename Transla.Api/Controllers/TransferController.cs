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
    public class TransferController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IDictionaryService _dictionaryService;
        private readonly ICultureService _cultureService;

        public TransferController(IApplicationService applicationService, IDictionaryService dictionaryService, ICultureService cultureService)
        {
            _applicationService = applicationService;
            _dictionaryService = dictionaryService;
            _cultureService = cultureService;
        }

        [HttpGet]
        public async Task<ActionResult<ExportContract>> Export()
        {
            try
            {
                return Ok(new ExportContract()
                {
                    Applications = await _applicationService.GetAll(),
                    Cultures = await _cultureService.GetAll(),
                    Dictionaries = await _dictionaryService.GetAll()
                });

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<ExportContract>> Import(ExportContract contract)
        {
            try
            {
                foreach (var culture in contract.Cultures ?? new List<CultureContract>())
                    await _cultureService.Save(culture.CultureName);

                foreach (var application in contract.Applications ?? new List<ApplicationContract>())
                    await _applicationService.Save(application.Alias);

                foreach (var dictionary in contract.Dictionaries ?? new List<DictionaryContract>())
                    await _dictionaryService.Save(dictionary);

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
