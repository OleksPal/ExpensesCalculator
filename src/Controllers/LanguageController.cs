using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ExpensesCalculator.Controllers
{
    public class LanguageController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            string? languageCookie = Request.Cookies["languageCulture"];

            if (languageCookie != null)
            {
                var languageManager = new LanguageManager();
                languageManager.ChangeLanguageCulture(languageCookie);

                Response.Cookies.Append("languageCulture", languageCookie);

                TempData["languageCulture"] = languageCookie;
            }
            else
            {
                TempData["languageCulture"] = "en-US";
            }
        }

        public IActionResult ChangeLanguage(string language, string returnUrl)
        {
            var languageManager = new LanguageManager();
            languageManager.ChangeLanguageCulture(language);

            Response.Cookies.Append("languageCulture", language);

            return Redirect(returnUrl);
        }
    }
}
