using TBS.PrintTest.Business.Models;
using System.Collections.Generic;

namespace TBS.PrintTest.Business.Services.Interfaces
{
    public interface ICodelookupService
    {
        List<Codelookup> GetProvinces(int id);

        List<Codelookup> GetTermTypes(int id);
    }
}
