using Microsoft.AspNetCore.Mvc;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Dto;

namespace ProjectOversight.API.Services.Interface
{
    public interface IAssetService
    {
        Task<List<Asset>> GetAssetsAsync();
        Task<bool> AddAsset(Asset asset);
        Task<List<AssetRequest>> GetAssetRequestAsync();
        Task<bool> AddAssetRequest(AssetRequest assetRequest);
    }
}
