﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    [Authorize]
    public class ReleaseNotesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IReleaseNotesService _releaseNotesService;
        public ReleaseNotesController(IUnitOfWork repository, UserManager<User> userManager,
                              IConfiguration configuration, IMapper mapper, SignInManager<User> signInManager,
                             IReleaseNotesService releaseNotesService)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _signInManager = signInManager;
            _releaseNotesService = releaseNotesService;
        }

        [HttpGet("GetReadyForUATTaskList")]
        public async Task<ActionResult<IEnumerable<Data.Model.Task>>> GetReadyForUATTaskList(int projectId)
        {
            try
            {
                var result = await _releaseNotesService.GetAllReadyForUATTasklist(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("UpdateInUATTaskList")]
        public async Task<ActionResult<IEnumerable<Data.Model.Task>>> UpdateInUATTaskList(List<int> projectId)
        {
            try
            {
                var result = await _releaseNotesService.UpdateInUATTask(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
