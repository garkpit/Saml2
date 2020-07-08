using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SampleAspNetCore2ApplicationNETFramework.Controllers
{
    public class JayController : Controller
    {
        [Route("[controller]/[action]")]
        public IActionResult Hello()
        {
            return new OkResult();
        }
    }
}
