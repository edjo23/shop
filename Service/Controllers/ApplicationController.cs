using Microsoft.AspNetCore.Mvc;
using Shop.Contracts.Entities;
using Shop.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Server.Controllers
{
    [Route("api/[controller]")]
    public class ApplicationController : Controller
    {
        public ApplicationController(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
        }

        private readonly IApplicationService ApplicationService;

        [HttpGet("denomination")]
        public IEnumerable<Denomination> GetDenominations()
        {
            return ApplicationService.GetDenominations();
        }

        [HttpGet("image")]
        public IEnumerable<string> GetImageList()
        {
            return ApplicationService.GetImageList();
        }

        [HttpGet("image/{code}")]
        public byte[] GetImage(string code)
        {
            return ApplicationService.GetImage(code);
        }

        [HttpGet("text/{code}")]
        public string[] GetText(string code)
        {
            return ApplicationService.GetText(code);
        }
    }
}
