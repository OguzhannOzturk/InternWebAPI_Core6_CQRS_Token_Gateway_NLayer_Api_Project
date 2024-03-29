using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Entities.Dto;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.Repository;

public class AdvertRepository:EfEntityRepositoryBase<Advert, AppDbContext>,IAdvertRepository
{
    public AdvertRepository(AppDbContext context) : base(context)
    {
    }


    public async Task<IEnumerable<AdvertWithAdvertDetailDTO>> GetAdvertList(int workplaceId)
    {
        var result = await (from advert in Context.Adverts
            join advertDetail in Context.AdvertDetails on advert.AdvertId equals advertDetail.AdvertId
            where advert.WorkplaceId == workplaceId && advert.DeleteDate==null
            select new AdvertWithAdvertDetailDTO()
            {
                AdvertId = advert.AdvertId,
                WorkplaceId = advert.WorkplaceId,
                CategoryId = advert.CategoryId,
                AdvertName = advert.AdvertName,
                AdvertSummary = advert.AdvertSummary,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate,
                Quota = advert.Quota,
                
                CompanyInfo = advertDetail.CompanyInfo,
                WorkDefinition = advertDetail.WorkDefinition,
                Quality = advertDetail.Quality,
                WorkEnvironment = advertDetail.WorkEnvironment,
                WorkHour = advertDetail.WorkHour,
                Facilities = advertDetail.Facilities,
                Wage = advertDetail.Wage,
                
                isActive = advert.isActive,
                CreatedDate = advert.CreatedDate,
                CreatedUserId = advert.CreatedUserId,
                UpdateDate = advert.UpdateDate,
                UpdateUserId = advert.UpdateUserId
            }).ToListAsync();
        return result;
    }

    public void AdvertWorkplaceControl(int workplaceId)
    {
        var result =  Query().Any(x => x.WorkplaceId == workplaceId && x.DeleteDate==null);
        if (!result)
        {
            throw new System.Exception("No ad was found for your business.");
        }
    }

    public void WorkplaceControl(int WorkplaceId)
    {
        var result = Context.Workplaces.Any(x => x.WorkplaceId == WorkplaceId && x.DeleteDate == null);
        if (!result)
        {
            throw new System.Exception("Job posting not found.");
        }
    }

    public void AdminWorkplaceControl(int workplaceId, Guid userId)
    {
        var result = Context.Workplaces.Any(x => x.WorkplaceId == workplaceId && x.AdminId == userId);
        if (!result)
        {
            throw new System.Exception("You are not authorized to perform this action");
        }
        
    }

    public void WorkplaceAdvertControl(int advertId, Guid userId)
    {
        var result = Query().Where(x => x.AdvertId == advertId ).First();
        var workplace = Context.Workplaces.Any(x => x.WorkplaceId == result.WorkplaceId && x.AdminId == userId);
        if (!workplace)
        {
            throw new System.Exception("You are not authorized to perform this action");
        }
    }

    public void AdvertControl(int advertId)
    {
        var result = Query().Any(x => x.AdvertId == advertId && x.DeleteDate==null);
        if (!result)
        {
            throw new System.Exception("Ad not found.");
        }
    }

    public  void AdvertWorkplaceActive(int advertId)
    {
        var advert =  Query().Where(x => x.AdvertId == advertId).First();
        var result = Context.Workplaces.Any(x => x.WorkplaceId == advert.WorkplaceId && x.DeleteDate == null);
        if (!result)
        {
            throw new System.Exception("Ad not found.");
        }
    }

    public void AdvertStartDateControl(DateTime startDate,DateTime endDate)
    {
        if (startDate<DateTime.Now.AddDays(-1))
        {
            throw new System.Exception("Announcement start date can be today at the earliest.");
        }

        if (endDate<=startDate)
        {
            throw new System.Exception("Announcement end date cannot be earlier than the start date.");
        }
    }

    public async Task<AdvertWithAdvertDetailDTO> GetAdvert(int advertId)
    {
        var result = await (from advert in Context.Adverts
            join advertDetail in Context.AdvertDetails on advert.AdvertId equals advertDetail.AdvertId
            where advert.AdvertId == advertId
            select new AdvertWithAdvertDetailDTO()
            {
                AdvertId = advert.AdvertId,
                WorkplaceId = advert.WorkplaceId,
                CategoryId = advert.CategoryId,
                AdvertName = advert.AdvertName,
                AdvertSummary = advert.AdvertSummary,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate,
                Quota = advert.Quota,
                
                CompanyInfo = advertDetail.CompanyInfo,
                WorkDefinition = advertDetail.WorkDefinition,
                Quality = advertDetail.Quality,
                WorkEnvironment = advertDetail.WorkEnvironment,
                WorkHour = advertDetail.WorkHour,
                Facilities = advertDetail.Facilities,
                Wage = advertDetail.Wage,
                
                isActive = advert.isActive,
                CreatedDate = advert.CreatedDate,
                CreatedUserId = advert.CreatedUserId,
                UpdateDate = advert.CreatedDate,
                UpdateUserId = advert.CreatedUserId
            }).FirstAsync();
        return result;
    }

    public async Task<IEnumerable<ActiveAdvertListDTO>> GetAdvertList()
    {
        var result = await (from advert in Context.Adverts
            join workplace in Context.Workplaces on advert.WorkplaceId equals workplace.WorkplaceId
            join user in Context.Users on workplace.AdminId equals user.UserId
            join advertCategory in Context.AdvertCategories on advert.CategoryId equals advertCategory.CategoryId
            where advert.DeleteDate==null &&
                  advert.isActive==true &&
                  workplace.DeleteDate==null &&
                  workplace.isActive == true&&
                  user.DeleteDate==null &&
                  user.isActive==true &&
                  advert.StartDate <= DateTime.Now &&
                  advert.EndDate >= DateTime.Now
                  select new ActiveAdvertListDTO()
            {
                AdvertId = advert.AdvertId,
                CategoryId = advert.CategoryId,
                WorkplaceId = advert.WorkplaceId,
                WorkplaceName = workplace.WorkplaceName,
                CategoryName = advertCategory.CategoryName,
                AdvertName = advert.AdvertName,
                AdvertSummary = advert.AdvertSummary,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate,
                Quota = advert.Quota,
                CreatedDate = advert.CreatedDate
                
            }).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ActiveAdvertListDTO>> GetAdvertInCategoryList(int categoryId)
    {
        var result = await (from advert in Context.Adverts
            join workplace in Context.Workplaces on advert.WorkplaceId equals workplace.WorkplaceId
            join user in Context.Users on workplace.AdminId equals user.UserId
            join advertCategory in Context.AdvertCategories on advert.CategoryId equals advertCategory.CategoryId
            where advert.DeleteDate==null &&
                  advert.isActive==true &&
                  workplace.DeleteDate==null &&
                  workplace.isActive == true&&
                  user.DeleteDate==null &&
                  user.isActive==true &&
                  advert.CategoryId ==categoryId &&
                  advert.StartDate <= DateTime.Now &&
                  advert.EndDate >= DateTime.Now
            select new ActiveAdvertListDTO()
            {
                AdvertId = advert.AdvertId,
                CategoryId = advert.CategoryId,
                WorkplaceId = advert.WorkplaceId,
                WorkplaceName = workplace.WorkplaceName,
                CategoryName = advertCategory.CategoryName,
                AdvertName = advert.AdvertName,
                AdvertSummary = advert.AdvertSummary,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate,
                Quota = advert.Quota,
                CreatedDate = advert.CreatedDate
                
            }).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<ActiveAdvertListDTO>> GetAdvertInWorkplaceList(int workplaceId)
    {
        var result = await (from advert in Context.Adverts
            join workplace in Context.Workplaces on advert.WorkplaceId equals workplace.WorkplaceId
            join user in Context.Users on workplace.AdminId equals user.UserId
            join advertCategory in Context.AdvertCategories on advert.CategoryId equals advertCategory.CategoryId
            where advert.DeleteDate==null &&
                  advert.isActive==true &&
                  workplace.DeleteDate==null &&
                  workplace.isActive == true&&
                  user.DeleteDate==null &&
                  user.isActive==true &&
                  advert.WorkplaceId == workplaceId &&
                  advert.StartDate <= DateTime.Now &&
                  advert.EndDate >= DateTime.Now
            select new ActiveAdvertListDTO()
            {
                AdvertId = advert.AdvertId,
                CategoryId = advert.CategoryId,
                WorkplaceId = advert.WorkplaceId,
                WorkplaceName = workplace.WorkplaceName,
                CategoryName = advertCategory.CategoryName,
                AdvertName = advert.AdvertName,
                AdvertSummary = advert.AdvertSummary,
                StartDate = advert.StartDate,
                EndDate = advert.EndDate,
                Quota = advert.Quota,
                CreatedDate = advert.CreatedDate
                
            }).ToListAsync();
        return result;
    }
    public int GetCategoryInAdvertCount(int categoryId)
    {
        bool control = Query().Any(x => x.CategoryId == categoryId);
        if (control)
        {
            var result = Query().Where(x => x.CategoryId == categoryId).Count(_=>_.CategoryId==categoryId);
            return result;
        }

        return 0;
    }
}