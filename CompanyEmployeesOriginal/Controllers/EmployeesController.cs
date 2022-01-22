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
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        {
            Company company = await HandleGetCompanyById(companyId);
            if (company == null)
            {
                return NotFound();
            }
            var empFromDb = await _repo.Employee.GetEmployeesAsync(companyId, trackChanges: false);
            var empDto = _mapper.Map<IEnumerable<EmployeeDto>>(empFromDb);
            return Ok(empDto);
        }

        [HttpGet("{id}", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployee(Guid companyId, Guid id)
        {
            Company company = await HandleGetCompanyById(companyId);

            if (company == null)
            {
                return NotFound();
            }
            Employee employeeForCompany = await HandleGetEmployeeById(companyId, id, trackChanges: true);
            if (employeeForCompany == null)
            {
                return NotFound();
            }
            EmployeeDto empDto = _mapper.Map<EmployeeDto>(employeeForCompany);
            return Ok(empDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeDto)
        {
            if (employeeDto == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, nameof(EmployeeForCreationDto));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForCreationDto object");
                return UnprocessableEntity(ModelState);
            }
            Company company = await HandleGetCompanyById(companyId);

            if (company == null)
            {
                return NotFound();
            }
            var employeeEntity = _mapper.Map<Employee>(employeeDto);

            _repo.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            await _repo.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id = employeeToReturn.Id
            }, employeeToReturn);

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid employeeId)
        {
            Company company = await HandleGetCompanyById(companyId);

            if (company == null)
            {
                return NotFound();
            }
            Employee employeeForCompany = await HandleGetEmployeeById(companyId, employeeId, trackChanges: true);
            if (employeeForCompany == null)
            {
                return NotFound();
            }

            _repo.Employee.DeleteEmployee(employeeForCompany);
            await _repo.SaveAsync();
            return NoContent();
        }

        [HttpPut("{employeeId}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid employeeId, [FromBody] EmployeeForUpdateDto employeeDto)
        {
            if (employeeDto == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.ObjectFromClientIsNull, nameof(EmployeeForUpdateDto));
                _logger.LogError(strToLog);
                return BadRequest(strToLog);
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
                return UnprocessableEntity(ModelState);
            }
            Company company = await HandleGetCompanyById(companyId);

            if (company == null)
            {
                return NotFound();
            }
            Employee employeeForCompany = await HandleGetEmployeeById(companyId, employeeId, trackChanges: true);

            if (employeeForCompany == null)
            {
                return NotFound();
            }

            _mapper.Map(employeeDto, employeeForCompany);
            await _repo.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(
            Guid companyId, 
            Guid id, 
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc
        )
        {

            if (HandleDtoFromBody(patchDoc) == null)
            {
                return BadRequest();
            }
            Company company = await HandleGetCompanyById(companyId);
            if (company == null)
            {
                return NotFound();
            }
            Employee employeeForCompany = await HandleGetEmployeeById(companyId, id, trackChanges: true);

            if (employeeForCompany == null)
            {
                return NotFound();
            }

            EmployeeForUpdateDto employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeForCompany);

            patchDoc.ApplyTo(employeeToPatch, ModelState);
            TryValidateModel(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document");
                return UnprocessableEntity(ModelState);
            }

            _mapper.Map(employeeToPatch, employeeForCompany);

            await _repo.SaveAsync();

            return NoContent();
        }

        [NonAction]
        public async Task<Company> HandleGetCompanyById(Guid companyId, bool trackChanges = false)
        {
            Company company = await _repo.Company.GetCompanyAsync(companyId, trackChanges);
            if (company == null)
            {
                string strToLog = string.Format(LoggerCustomMessages.IdNotFoundInDB, nameof(Company), companyId);
                _logger.LogInfo(strToLog);
            }
            return company;
        }
        [NonAction]
        public async Task<Employee> HandleGetEmployeeById(Guid companyId, Guid employeeId, bool trackChanges = false)
        {
            Employee employeeForCompany = await _repo.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges);
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
