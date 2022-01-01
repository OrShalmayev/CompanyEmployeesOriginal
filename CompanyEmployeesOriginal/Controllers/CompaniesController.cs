using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CompanyEmployeesOriginal.DTO.Company;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    public class CompaniesController : BaseApiController
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager r, IMapper m)
        {
            _repo = r;
            _mapper = m;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repo.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }
    }
}
