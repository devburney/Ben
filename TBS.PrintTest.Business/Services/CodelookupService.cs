using AutoMapper;
using TBS.PrintTest.Business.Models;
using TBS.PrintTest.Business.Services.Interfaces;
using TBS.PrintTest.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace TBS.PrintTest.Business.Services
{
    public class CodelookupService : ICodelookupService
    {
        private readonly IMapper _mapper;
        private readonly ICodelookupRepository _codelookupRepository;

        public CodelookupService(ICodelookupRepository codelookupRepository, IMapper mapper)
        {
            _codelookupRepository = codelookupRepository ?? throw new ArgumentNullException(nameof(codelookupRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public List<Codelookup> GetProvinces(int id)
        {
            return _mapper.Map<List<Codelookup>>(_codelookupRepository.GetProvinces(id));
        }

        public List<Codelookup> GetTermTypes(int id)
        {
            return _mapper.Map<List<Codelookup>>(_codelookupRepository.GetTermTypes(id));
        }
    }
}
