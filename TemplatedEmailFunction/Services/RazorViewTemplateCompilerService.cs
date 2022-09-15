using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace TemplatedEmailFunction.Services
{
    public class RazorViewTemplateCompilerService
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewTemplateCompilerService(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceProvider serviceProvider)
        {
            this._viewEngine = viewEngine;
            this._tempDataProvider = tempDataProvider;
            this._serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Renders a Razor view and model to a string.
        /// The name of the view is automatically inferred from the model name (e.g. IndexModel -> Index).
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="templateModelName"></param>
        /// <returns></returns>
        public async Task<string> RenderViewToStringAsync<TModel>(TModel model, string templateModelName)
        {
            string templateName = templateModelName.Replace("Model", string.Empty);

            ActionContext actionContext = GetActionContext();
            IView view = FindView(actionContext, templateName);

            using StringWriter output = new StringWriter();
            ViewDataDictionary<TModel> viewDataDictionary = new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(), modelState: new ModelStateDictionary())
            {
                Model = model
            };
            TempDataDictionary tempDataDictionary = new TempDataDictionary(actionContext.HttpContext, _tempDataProvider);
            ViewContext viewContext = new ViewContext(actionContext, view, viewDataDictionary, tempDataDictionary, output, new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            ViewEngineResult getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success) return getViewResult.View;

            ViewEngineResult findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success) return findViewResult.View;

            System.Collections.Generic.IEnumerable<string> searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            string errorMessage = string.Join(Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));
            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            DefaultHttpContext httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}
