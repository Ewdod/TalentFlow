using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.CompanyManager.Controllers
{
    [Authorize(Roles = "CompanyManager")]
    [Area("CompanyManager")]
    public abstract class CompanyManagerBaseController : Controller
    {
    
    }
}
