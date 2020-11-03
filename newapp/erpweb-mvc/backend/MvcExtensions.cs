using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Text;

namespace backend.Extensions
{
    public static class MvcExtensions
    {
        public static MvcHtmlString TextBoxOrLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool readOnly = false, bool generateHidden = false, object htmlAttributes = null)
        { 
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            return (readOnly ? Concat(MvcHtmlString.Create(metadata.Model.ToString()), (generateHidden ? helper.HiddenFor(expression) : MvcHtmlString.Empty)) : helper.TextBoxFor(expression,htmlAttributes));
        }

        public static MvcHtmlString TextAreaOrLabelFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, bool readOnly = false, object htmlAttributes = null)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);
            return (readOnly ? MvcHtmlString.Create(metadata.Model.ToString()) : helper.TextAreaFor(expression, htmlAttributes));
        }

        public static MvcHtmlString TimePeriodFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> helper,
                                                                                  Expression<Func<TModel, TProperty1>>
                                                                                      expressionFrom,
                                                                                  Expression<Func<TModel, TProperty2>>
                                                                                      expressionTo,
                                                                                  object htmlAttributes = null)
        {
            return Concat(MvcHtmlString.Create(" from "),
                          helper.DropDownListFor(expressionFrom,
                                                 new SelectList(WebUtilities.GetOpeningStoreTimes()), htmlAttributes),
                          MvcHtmlString.Create(" to "),
                          helper.DropDownListFor(expressionTo,
                                                 new SelectList(
                                                     WebUtilities.GetClosingStoreTimes()), htmlAttributes));
        }

        //public static MvcHtmlString TimePeriodFor<TModel, TProperty1,TProperty2>(this HtmlHelper<TModel> helper,
        //                                                                          Expression<Func<TModel, TProperty1>> expressionHour, 
        //                                                                          Expression<Func<TModel, TProperty2>> expressionMinute, 
        //                                                                          string label = "", bool includeClosed = true,                                                                          
        //                                                                          object htmlAttributes = null, IEnumerable<int> hours = null, IEnumerable<int> minutes = null)
        //{

        //    List<int> defaultHours = new List<int>();
        //    List<int> defaultMinutes = new List<int>();
        //    List<SelectListItem> hourItems = new List<SelectListItem>(), minuteItems = new List<SelectListItem>();
        //    for (int i = 0; i < 24; i++) {
        //        defaultHours.Add(i);
        //    }
        //    defaultMinutes.AddRange(new int[] { 0, 15, 30, 45 });
        //    hourItems = (hours ?? defaultHours).Select(i => new SelectListItem { Id = i.ToString("00"), Title = i.ToString("00") }).ToList();
        //    if (includeClosed)
        //        hourItems.Add(new SelectListItem { Id = "closed", Title = "closed" });
        //    minuteItems = (minutes ?? defaultMinutes).Select(i => new SelectListItem { Id = i.ToString("00"), Title = i.ToString("00") }).ToList();

        //    return Concat(MvcHtmlString.Create(label),
        //                  helper.DropDownListFor(expressionHour,
        //                                         new SelectList(hourItems,"Id","Title"), htmlAttributes),
        //                  helper.DropDownListFor(expressionMinute,
        //                                         new SelectList(minuteItems,"Id","Title"), htmlAttributes));
        //}

        public static MvcHtmlString TimePeriodFor<TModel, TProperty>(this HtmlHelper<TModel> helper,
                                                                                  Expression<Func<TModel, TProperty>> expression,                                                                                  
                                                                                  string label = "", bool includeClosed = true,
                                                                                  object htmlAttributes = null, IEnumerable<int> hours = null, IEnumerable<int> minutes = null)
        {
            List<int> defaultHours = new List<int>();
            List<int> defaultMinutes = new List<int>();
            
            for (int i = 0; i < 24; i++) {
                defaultHours.Add(i);
            }
            defaultMinutes.AddRange(new int[] { 0, 15, 30, 45 });
            var value = string.Empty + ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var openClose = OpenCloseTime.FromHourMinute(value.ToString());
            var name = ExpressionHelper.GetExpressionText((LambdaExpression)expression);
            return Concat(MvcHtmlString.Create(label),helper.HiddenFor(expression),
                          helper.DropDownList(name + "_ddlHour",
                                                 new SelectList((hours ?? defaultHours).Select(i=>i.ToString("00")).Union(new[] { "closed" }),openClose.Hour.ToLower()), htmlAttributes),
                          helper.DropDownList(name + "_ddlMinute",
                                                 new SelectList((minutes ?? defaultMinutes).Select(i => i.ToString("00")),openClose.Minute), htmlAttributes));
        }

        private static MvcHtmlString Concat(params MvcHtmlString[] items)
        {
            var sb = new StringBuilder();
            foreach (var item in items.Where(i => i != null))
                sb.Append(item.ToHtmlString());
            return MvcHtmlString.Create(sb.ToString());
        }

        public static HtmlString GlobalizationScript(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<script src=\"/Scripts/globalize/cultures/globalize.culture.en-GB.js\"></script>");
            sb.AppendLine("<script type='text/javascript' >");
            sb.AppendLine("Globalize.culture('en-GB');");
            sb.AppendLine("$.global = Globalize;");
            sb.AppendLine("</script>");
            return new HtmlString(sb.ToString());
        }
        
    }

    public class SelectListItem
    {
        public object Id { get; set; }
        public string Title { get; set; }
    }

    public class OpenCloseTime
    {
        public string Hour { get; set; }
        public string Minute { get; set; }

        public OpenCloseTime()
        {
        }

        public OpenCloseTime(string hourMinute)
        {
            FromHourMinute(hourMinute);
        }

        public static OpenCloseTime FromHourMinute(string hourMinute)
        {
            var result = new OpenCloseTime();
            var parts = hourMinute.Split(':');
            result.Hour = parts[0];
            if (parts.Length > 1)
                result.Minute = parts[1];
            return result;
        }

        public string ToHourMinute()
        {
            return Hour + ":" + Minute;
        }
    }
    public static class HtmlRequestHelper
    {
        public static string Id(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("id"))
                return (string)routeValues["id"];
            else if (HttpContext.Current.Request.QueryString.AllKeys.Contains("id"))
                return HttpContext.Current.Request.QueryString["id"];

            return string.Empty;
        }

        public static string ControllerName(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string ActionName(this HtmlHelper htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }
    }
}