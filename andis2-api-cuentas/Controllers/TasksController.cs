using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using andis2_api_cuentas.Types;
using andis2_api_cuentas.Handlers;
using andis2_api_cuentas.Services;

namespace andis2_api_cuentas.Controllers;
    
[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly AccountContext _context;
    private readonly ITaskQueue _taskQueue;
    private readonly IStatusService _statusService;

    public TasksController(AccountContext context, ITaskQueue queue, IStatusService statusService)
    {
        _context = context;
        _taskQueue = queue;
        _statusService = statusService;
    }

    // GET: api/Tasks/{guid}
    [HttpGet("{id}")]
    public ActionResult GetStatus(Guid id)
    {
        var status = _statusService.GetStatus(id);
        return Ok(status);
    }
}
