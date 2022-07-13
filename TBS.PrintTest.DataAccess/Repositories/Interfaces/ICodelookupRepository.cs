using TBS.PrintTest.DataAccess.Models;
using System.Collections.Generic;

namespace TBS.PrintTest.DataAccess.Repositories.Interfaces
{
    public interface ICodelookupRepository
    {
        List<Province> GetProvinces(int id);

        List<EmployeeTermType> GetTermTypes(int id);
    }
}
