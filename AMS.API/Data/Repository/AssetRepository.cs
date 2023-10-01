using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;

namespace ProjectOversight.API.Data.Repository
{
    public class AssetRepository:Repository<Asset>, IAssetRepository
    {
        public AssetRepository(ProjectOversightContext projectOversightContext):base(projectOversightContext) 
        { 
        
        }
    }
}
