using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAPA.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            else
            {
                //Check Authorize
                /*
                    --Example--
                    if(Role == "Super Admin"){
                        //Dapat akses seluruh controller Employee, Category DLL
                    }else if(Role == "Admin"){
                        //Hanya boleh akses controller employee
                    }else{
                         //Hanya boleh akses controller category
                    }
                */
            }           
        }
    }
}
