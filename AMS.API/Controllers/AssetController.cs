using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Controllers
{
    [Route("v1/app/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IMapper _mapper;

        public AssetController(IAssetService assetService, IMapper mapper)
        {
            _assetService = assetService;
            _mapper = mapper;
        }

        [HttpGet("GetAsset")]
        public async Task<ActionResult<List<Asset>>> GetAssets()
        {
            try
            {
                var assets = await _assetService.GetAssetsAsync();
              
                return Ok(assets);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost("AddAsset")]
        public async Task<ActionResult<bool>> AddAsset(Asset asset)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _assetService.AddAsset(asset);

                if (result)
                { 
                    return Ok(true);
                }
                else
                {
                    return Conflict("An asset with the same AssetCode already exists in the database.");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetAssetRequest")]
        public async Task<ActionResult<List<AssetRequest>>> GetAssetRequest()
        {
            try
            {
                var AssetRequest = await _assetService.GetAssetRequestAsync();

                return Ok(AssetRequest);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("AddAssetRequest")]
        public async Task<ActionResult<bool>> AddAssetRequest(AssetRequest assetRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _assetService.AddAssetRequest(assetRequest);

                if (result)
                {
                    return Ok(true);
                }
                else
                {
                    return Conflict("An asset with the same AssetRequest already exists in the database.");
                }
            }
            catch (Exception ex) 
            {
            throw;
            }
        }

    }
}