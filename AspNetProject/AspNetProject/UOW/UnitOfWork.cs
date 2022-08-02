using StudentAdminPortal.API.DataModels;
using StudentAdminPortal.API.Repositories;

namespace AspNetProject.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private StudentAdminContext context;
        public UnitOfWork(StudentAdminContext context)
        {
            this.context = context;
            Student = new StudentRepository(this.context);
        }
        public IStudentRepository Student
        {
            get;
            private set;
        }
       
        public void Dispose()
        {
            context.Dispose();
        }
        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
