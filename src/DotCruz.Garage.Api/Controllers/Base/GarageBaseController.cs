using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Shared.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotCruz.Garage.Api.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
[Authorize(Policy = SecurityPolicies.AdminOnly)]
public class GarageBaseController : ControllerBase;
