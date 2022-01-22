using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployeesOriginal.ActionFilters;
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

        public CompaniesController(IRepositoryManager r, ILoggerManager l , IMapper m)
        {
            _repo = r;
            _logger = l;
            _mapper = m;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repo.Company.GetAllCompaniesAsync(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        [HttpGet("{id}", Name = nameof(GetCompany))]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var c = await _repo.Company.GetCompanyAsync(id, trackChanges: false);
            if (c == null)
            {
                var strToLog = string.Format(LoggerCustomMessages.IdNotFoundInDB, nameof(Company), id);
                _logger.LogInfo(strToLog);
                return NotFound();
            } else
            {
                var companyDto = _mapper.Map<CompanyDto>(c);
                return Ok(companyDto);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            Company companyEntity = _mapper.Map<Company>(company);
            
            _repo.Company.CreateCompany(companyEntity);
            await _repo.SaveAsync();

            CompanyDto companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute(nameof(GetCompany), new { id = companyToReturn.Id }, companyToReturn);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([FromRoute][ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            var parameterName = nameof(ids);
            if (ids == null)
            {
                var strToLog = string.Format(LoggerCustomMessages.ParameterIsNull, parameterName);
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }
            var companyEntities = await _repo.Company.GetByIdsAsync(ids, trackChanges: false);
            if (ids.Count() != companyEntities.Count())
            {
                string strToLog = string.Format(LoggerCustomMessages.CollectionParametersIsNull, parameterName);
                _logger.LogError(strToLog);
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.CollectionFromClientIsNull, nameof(Company));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }

            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repo.Company.CreateCompany(company);
            }

            await _repo.SaveAsync();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id)).ToList();

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            Company c = HttpContext.Items["company"] as Company;
            
            _repo.Company.DeleteCompany(c);
            await _repo.SaveAsync();

            return NoContent();
        }

        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto companyDto)
        {
            Company c = HttpContext.Items["company"] as Company;
            _mapper.Map(companyDto, c);
            await _repo.SaveAsync();

            return NoContent();
        }
    }
}
