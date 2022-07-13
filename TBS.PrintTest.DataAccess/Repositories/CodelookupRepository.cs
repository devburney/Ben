using TBS.PrintTest.DataAccess.Models;
using TBS.PrintTest.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TBS.PrintTest.DataAccess.Repositories
{
    public class CodelookupRepository : ICodelookupRepository
    {
        private readonly IMTDTemplateContext _context;

        public CodelookupRepository(IMTDTemplateContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public List<Province> GetProvinces(int id)
        {
            var provinces = _context.Province.AsNoTracking().Where(x => x.ProvinceActiveBoolean == true).OrderBy(x => x.ProvinceSortOrder).ToList();

            // check to see if provided id is in the list.  if it isn't, it's probably an inactive codelookup so retreive it separately.
            if (!provinces.Any(x => x.ProvinceId == id))
            {
                // get the deleted code.
                var deletedProvince = _context.Province.AsNoTracking().FirstOrDefault(x => x.ProvinceId == id);

                if (deletedProvince != null) provinces.Add(deletedProvince);
            }

            return provinces.ToList();
        }

        public List<EmployeeTermType> GetTermTypes(int id)
        {
            var termTypes = _context.EmployeeTermType.AsNoTracking().Where(x => x.EmployeeTermTypeActiveBoolean == true).OrderBy(x => x.EmployeeTermTypeSortOrder).ToList();

            // check to see if term type id is in the list.  if it isn't, it's probably an inactive codelookup so retreive it separately.
            if (!termTypes.Any(x => x.EmployeeTermTypeId == id))
            {
                // get the deleted code.
                var deletedTermType= _context.EmployeeTermType.AsNoTracking().FirstOrDefault(x => x.EmployeeTermTypeId == id);

                if (deletedTermType != null) termTypes.Add(deletedTermType);
            }

            return termTypes.ToList();
        }
    }
}
