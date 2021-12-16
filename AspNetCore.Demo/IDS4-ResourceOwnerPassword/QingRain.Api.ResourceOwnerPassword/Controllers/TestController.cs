// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace QingRain.Api.ResourceOwnerPassword
{
    [Route("test")]
    [Authorize]
    public class TestController : ControllerBase
    {
        [Authorize(Policy = "myPolicy")]
        //[Authorize(Roles = "admin")]
        public IActionResult Get()
        {
            string? token = HttpContext.GetTokenAsync("access_token").Result;

            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}