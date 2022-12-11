namespace HospitalWeb.Services.Interfaces
{
    public interface IDbInitializer
    {
        public Task SetupRoles();
        public Task CreateSuperAdmin();
        public Task GenerateDb();
        public Task Init();
    }
}
