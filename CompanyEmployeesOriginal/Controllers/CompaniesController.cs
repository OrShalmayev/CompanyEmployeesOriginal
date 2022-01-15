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

        public CompaniesController(IRepositoryManager r, ILoggerManager l , IMapper m)
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
        public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company == null)
            {
                var strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, nameof(CompanyForCreationDto));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
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
            var parameterName = nameof(ids);
            if (ids == null)
            {
                var strToLog = string.Format(LoggerCustomMessages.ParameterIsNull, parameterName);
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }
            var companyEntities = _repo.Company.GetByIds(ids, trackChanges: false);
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
        public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
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

            _repo.Save();

            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id)).ToList();

            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany(Guid id)
        {
            Company c = HandleGetComapnyById(companyId: id);
            
            if (c == null)
            {
                return NotFound();
            }
            
            _repo.Company.DeleteCompany(c);
            _repo.Save();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto companyDto)
        {
            if (HandleDtoFromBody(companyDto) == null)
            {
                return BadRequest();
            }

            Company c = HandleGetComapnyById(id, trackChanges: true);
            if (c == null)
            {
                return NotFound();
            }

            _mapper.Map(companyDto, c);
            _repo.Save();

            return NoContent();
        }

        [NonAction]
        public Company HandleGetComapnyById(Guid companyId, bool trackChanges = false)
        {
            Company c = _repo.Company.GetCompany(companyId, trackChanges);
            if (c == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.IdNotFoundInDB, nameof(Company), companyId);
                _logger.LogError(strToLog);
            }
            return c;
        }

        [NonAction]
        public object HandleDtoFromBody(object dto)
        {
            if (dto == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, dto.GetType().Name);
                _logger.LogError(strToLog);

            }
            return dto;
        }
    }
}
