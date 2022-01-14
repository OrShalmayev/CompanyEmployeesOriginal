using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CompanyEmployeesOriginal.DTO.Company;
using CompanyEmployeesOriginal.Entities.ModelBinders;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    [ApiController]
    [Route("/api/companies")]
    public class CompaniesController : ControllerBase
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

        [HttpGet("{id}", Name = nameof(GetCompany))]
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

        [HttpPost]
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company == null)
            {
                _logger.LogError("CompanyForCreationDto object sent from client is null.");
                return BadRequest("CompanyForCreationDto object is null");
            }
            Company companyEntity = _mapper.Map<Company>(company);
            
            _repo.Company.CreateCompany(companyEntity);
            _repo.Save();

            CompanyDto companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute(nameof(GetCompany), new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public IActionResult GetCompanyCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = _repo.Company.GetByIds(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repo.Company.CreateCompany(company);
            }
            _repo.Save();
            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id)).ToList();
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }
    }
}
