using StudentAdminPortal.API.Repositories;

namespace AspNetProject.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Student
        {
            get;
        }
       
        int Save();
    }
}
