using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;

namespace DataAccess.Abstract;

public interface ICurrentRepository:IEntityRepository<User>
{
    Guid UserId();
    void AdminControl(Guid userId);
    void UserControl(Guid userId);
}