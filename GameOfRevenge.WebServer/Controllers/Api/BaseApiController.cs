using GameOfRevenge.WebServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GameOfRevenge.Common.Net;
using GameOfRevenge.Common.Models;

namespace GameOfRevenge.WebServer.Controllers.Api
{
    //[ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(AuthenticationSchemes = AuthHelper.AuthenticationSchemeJwt)]
    public abstract class BaseApiController : ControllerBase
    {
        protected Player Token => HttpContext.GetUser();

        protected IActionResult ReturnResponse(object response)
        {
            return Ok(response);
        }

        protected IActionResult ReturnResponse()
        {
            var response = new Response(CaseType.Success, "Success");
            return ReturnResponse(response);
        }

        protected IActionResult ReturnResponse(Response response)
        {
            if (response == null)
            {
                response = new Response(CaseType.Error, "Unexpected error has occured, please contact support team");
                return StatusCode(500, response);
            }
            else
            {
                if (response.GetCaseType() == CaseType.Error) return StatusCode(500, response);
                else if (response.GetCaseType() == CaseType.Success) return Ok(response);
                else if (response.GetCaseType() == CaseType.Invalid) return BadRequest(response);
                else return Unauthorized(response);
            }
        }

        protected IActionResult ReturnResponse<T>(Response<T> response)
        {
            if (response == null)
            {
                response = new Response<T>(CaseType.Error, "Unexpected error has occured, please contact support team");
                return StatusCode(500, response);
            }
            else
            {
                if (response.GetCaseType() == CaseType.Error) return StatusCode(500, response);
                else if (response.GetCaseType() == CaseType.Success) return Ok(response);
                else if (response.GetCaseType() == CaseType.Invalid) return BadRequest(response);
                else return Unauthorized(response);
            }
        }
    }
}
