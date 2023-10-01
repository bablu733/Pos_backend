using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;

namespace ProjectOversight.API.Data.Repository
{
    public class AssetRequestRepository:Repository<AssetRequest>,IAssetRequestRepository
    {
        public  AssetRequestRepository(ProjectOversightContext projectOversightContext) : base(projectOversightContext) 
        {

        }
    }
}
