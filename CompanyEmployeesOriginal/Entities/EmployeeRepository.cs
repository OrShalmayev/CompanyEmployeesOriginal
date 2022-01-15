using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Entities
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repoCtx) : base(repoCtx)
        {
        }

        public IEnumerable<Employee> GetEmployees(Guid id, bool trackChanges)
        {
            return FindByCondition(e => e.CompanyId.Equals(id), trackChanges)
                .OrderBy(e => e.Name);
        }

        public Employee GetEmployee(Guid companyId, Guid empId, bool trackChanges)
        {
            return FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(empId), trackChanges).SingleOrDefault();
        }
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }
    }
}
