using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ApplicationCore.Constants.AuthorizationConstants;

namespace Web.Areas.Personnel.Controllers
{
    [Authorize(Roles = "Personnel")]
    [Area("Personnel")]
    public class PersonnelBaseController : Controller
    {

    }
}
