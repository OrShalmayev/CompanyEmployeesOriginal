using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CompanyEmployeesOriginal.DTO.Company;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    public class CompaniesController : BaseApiController
    {
        private readonly IRepositoryManager _repo;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager r,ILoggerManager l ,IMapper m)
        {
            _repo = r;
            _logger = l;
            _mapper = m;
        }

        [HttpGet]
        public IActionResult GetCompanies()
        {
            var companies = _repo.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetCompany(Guid id)
        {
            var c = _repo.Company.GetCompany(id, trackChanges: false);
            if (c == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            } else
            {
                var companyDto = _mapper.Map<CompanyDto>(c);
                return Ok(companyDto);
            }
        }
    }
}
