using ProjectOversight.API.Data.Model;

namespace ProjectOversight.API.Services.Interface
{
    public interface ICommonMasterService
    {
        Task<List<CommonMaster>> GetCodeTableList();
    }
}
