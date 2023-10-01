using Microsoft.EntityFrameworkCore;
using ProjectOversight.API.Data;
using ProjectOversight.API.Data.Model;
using ProjectOversight.API.Data.Repository.Interface;
using ProjectOversight.API.Services.Interface;

namespace ProjectOversight.API.Services
{
    public class CommonMasterService : ICommonMasterService
    {

        private readonly ProjectOversightContext _dbContext;
        public CommonMasterService(ProjectOversightContext dbContext)
        {

            _dbContext = dbContext;
        }
        public async Task<List<CommonMaster>> GetCodeTableList()
        {
            var codeTable = await _dbContext.CommonMaster.ToListAsync();
            return codeTable;
        }
    }
}

