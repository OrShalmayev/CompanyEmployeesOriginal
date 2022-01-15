using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployeesOriginal.DTO.Employee;
using Contracts;
using Entities;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    [ApiController]
    [Route("/api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repo;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeesController(IRepositoryManager r, ILoggerManager l, IMapper m)
        {
            _repo = r;
            _logger = l;
            _mapper = m;
        }

        [HttpGet]
        public IActionResult GetEmployeesForCompany(Guid companyId)
        {
            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }
            var empFromDb = _repo.Employee.GetEmployees(companyId, trackChanges: false);
            var empDto = _mapper.Map<IEnumerable<EmployeeDto>>(empFromDb);
            return Ok(empDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployee(Guid companyId, Guid id)
        {
            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }
            if (HandleGetEmployeeById(companyId, id, out Employee employeeForCompany, trackChanges: true) == null)
            {
                return NotFound();
            }
            EmployeeDto empDto = _mapper.Map<EmployeeDto>(employeeForCompany);
            return Ok(empDto);
        }

        [HttpPost]
        public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeDto)
        {
            if (employeeDto == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, nameof(EmployeeForCreationDto));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }
            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employeeDto);

            _repo.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repo.Save();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id = employeeToReturn.Id
            }, employeeToReturn);

        }

        [HttpDelete]
        public IActionResult DeleteEmployee(Guid companyId, Guid employeeId)
        {
            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }

            if (HandleGetEmployeeById(companyId, employeeId, out Employee employeeForCompany, trackChanges: true) == null)
            {
                return NotFound();
            }

            _repo.Employee.DeleteEmployee(employeeForCompany);
            _repo.Save();
            return NoContent();
        }

        [HttpPut("{employeeId}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employeeDto)
        {
            if (employeeDto == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, nameof(EmployeeForUpdateDto));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }

            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }
            if (HandleGetEmployeeById(companyId, employeeId, out Employee employeeForCompany, trackChanges: true) == null)
            {
                return NotFound();
            }

            _mapper.Map(employeeDto, employeeForCompany);
            _repo.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(
            Guid companyId, 
            Guid id, 
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc
        )
        {
            if (HandleDtoFromBody(patchDoc) == null)
            {
                return BadRequest();
            }

            if (HandleGetCompanyById(companyId, out Company company) == null)
            {
                return NotFound();
            }

            if (HandleGetEmployeeById(companyId: companyId, employeeId: id, out Employee employeeForCompany, trackChanges: true) == null)
            {
                return NotFound();
            }

            EmployeeForUpdateDto employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeForCompany);

            patchDoc.ApplyTo(employeeToPatch);

            _mapper.Map(employeeToPatch, employeeForCompany);

            _repo.Save();

            return NoContent();
        }

        [NonAction]
        public Company HandleGetCompanyById(Guid companyId, out Company company, bool trackChanges = false)
        {
            company = _repo.Company.GetCompany(companyId, trackChanges);
            if (company == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.IdNotFoundInDB, nameof(Company), companyId);
                _logger.LogInfo(strToLog);
            }
            return company;
        }
        [NonAction]
        public Employee HandleGetEmployeeById(Guid companyId, Guid employeeId, out Employee employeeForCompany, bool trackChanges = false)
        {
            employeeForCompany = _repo.Employee.GetEmployee(companyId, employeeId, trackChanges);
            if (employeeForCompany == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.IdNotFoundInDB, nameof(Employee), employeeId);
                _logger.LogInfo(strToLog);
            }
            return employeeForCompany;
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
