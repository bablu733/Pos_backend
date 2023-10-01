namespace ProjectOversight.API.Services.Interface
{
    public interface IReleaseNotesService
    {
        Task<List<Data.Model.Task>> GetAllReadyForUATTasklist(int projectId);
        Task<List<Data.Model.Task>> UpdateInUATTask(List<int> projectId);
    }
}
