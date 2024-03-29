using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.Dto;

namespace DataAccess.Abstract;

public interface IWorkplaceRepository:IEntityRepository<Workplace>
{
    void WorkplaceControl(int workplaceId);
    Task<IEnumerable<Workplace>> GetNotDeletedWorkplace(Guid userId);
    void GetAdminControl(Guid userId);
    void AdminWordplaceControl(int wordplaceId, Guid userId);
    Task<IEnumerable<WorkplaceDTO>> GetWorkplaceAndAdminControl();

}