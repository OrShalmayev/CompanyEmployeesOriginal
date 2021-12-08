using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployeesOriginal.Controllers
{
    public class CompanyController : BaseApiController
    {
        [HttpGet]
        public string GetCompanies()
        {
            return "test";
        }
    }
}