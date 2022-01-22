using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace Entities
{
    public class RepositoryManager : IRepositoryManager
    {
        private RepositoryContext _repoCtx;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;
        public RepositoryManager(RepositoryContext repoCtx)
        {
            _repoCtx = repoCtx;
        }

        public ICompanyRepository Company
        {
            get
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_repoCtx);
                return _companyRepository;
            }
        }
        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new EmployeeRepository(_repoCtx);
                return _employeeRepository;
            }
        }
        public async Task SaveAsync() => await _repoCtx.SaveChangesAsync();
    }
}
