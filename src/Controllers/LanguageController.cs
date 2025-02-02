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
                ChangeLanguage(languageCookie);
            }
        }

        public void ChangeLanguage(string language)
        {
            var languageManager = new LanguageManager();
            languageManager.ChangeLanguageCulture(language);

            Response.Cookies.Append("languageCulture", language);
        }
    }
}
