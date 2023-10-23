using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApplicationCore.Constants.AuthorizationConstants;

namespace Web.Areas.Manager.Controllers
{
    [Authorize(Roles = "Manager")]
    [Area("Manager")]
    public abstract class ManagerBaseController : Controller
    {
      
    }
}
