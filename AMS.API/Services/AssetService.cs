using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Dto;
using ProjectOversight.API.Services.Interface;


namespace ProjectOversight.API.Services
{
    public class AssetService : IAssetService
    {

        private readonly ProjectOversightContext _dbContext;
        private readonly IUnitOfWork _repository;
       


        public AssetService(ProjectOversightContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Asset>> GetAssetsAsync()
        {
            try
            {
                var Asset = await _dbContext.Asset.ToListAsync();
                return Asset;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> AddAsset(Asset asset)
        {
            try
            {
                bool assetExists = await _dbContext.Asset.AnyAsync(a => a.AssetCode == asset.AssetCode);

                if (assetExists)
                {
                    return false;
                }
                Asset newAsset = new Asset()
                {
                    Id = asset.Id,
                    AssetCode = asset.AssetCode,
                    AssetType = asset.AssetType,
                    Specification = asset.Specification,
                    Description = asset.Description,
                    CreatedDate = asset.CreatedDate,
                    CreatedBy = asset.CreatedBy,
                    UpdatedDate = asset.UpdatedDate,
                    UpdatedBy = asset.UpdatedBy
                };

                _dbContext.Asset.Add(newAsset);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<AssetRequest>> GetAssetRequestAsync()
        {
            try
            {
                var AssetRequest = await _dbContext.AssetRequest.ToListAsync();

                return AssetRequest;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> AddAssetRequest(AssetRequest assetRequest)
        {
            try
            {
                bool assetRequestExists = await _dbContext.AssetRequest.AnyAsync(ar => ar.EmployeeId == assetRequest.EmployeeId);
                if (assetRequestExists)
                {
                    return false;
                }

                AssetRequest newAssetRequest = new AssetRequest()
                {
                    Id = assetRequest.Id,
                    EmployeeId = assetRequest.EmployeeId,
                    TypeOfRequest = assetRequest.TypeOfRequest,
                    AssetType = assetRequest.TypeOfRequest,
                    Specification = assetRequest.Specification,
                    Status = assetRequest.Status,
                    CreatedDate = assetRequest.CreatedDate,
                    CreatedBy = assetRequest.CreatedBy,
                    UpdatedDate = assetRequest.UpdatedDate,
                    UpdatedBy = assetRequest.UpdatedBy,
                };
                _dbContext.AssetRequest.Add(newAssetRequest);
                _dbContext.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
