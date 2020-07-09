using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleAspNetCore2ApplicationNETFramework.Data;

namespace SampleAspNetCore2ApplicationNETFramework.Controllers
{
    //[Route("[controller]/[action]")]
    public class JayController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public JayController(SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }


        public IActionResult Hello()
        {
            var req = HttpContext.Request;

            var user = HttpContext.User;


            return new OkResult();
        }


        public IActionResult ByTenant(string tenantId)
        {

            return new OkResult();
        }
    }
}
