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
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    [ApiController]
    [Route("/api/companies/{companyId}/employees")]
    public class EmployeesController: ControllerBase
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
            var comp = _repo.Company.GetCompany(companyId, trackChanges: false);
            if (comp == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            var empFromDb = _repo.Employee.GetEmployees(companyId, trackChanges: false);
            var empDto = _mapper.Map<IEnumerable<EmployeeDto>>(empFromDb);
            return Ok(empDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployee(Guid companyId, Guid id)
        {
            Company comp = _repo.Company.GetCompany(companyId, trackChanges: false);
            if (comp == null)
            {
                _logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }
            Employee empFromDb = _repo.Employee.GetEmployee(companyId, id, trackChanges: false);
            if (empFromDb == null)
            {
                _logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            EmployeeDto empDto = _mapper.Map<EmployeeDto>(empFromDb);
            return Ok(empDto);
        }
    }
}
