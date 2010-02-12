using System;
using System.Linq;

namespace N2.Web.Mvc
{
	/// <summary>
	/// Helps the N2's MVC route handler to deal with actions in url.
	/// </summary>
	public class ActionResolver : IPathFinder
	{
		private readonly IControllerMapper _controllerMapper;
		private readonly string[] _methods;
		//private const string DefaultAction = "index";

		public ActionResolver(IControllerMapper controllerMapper, string[] methods)
		{
			_controllerMapper = controllerMapper;
			_methods = methods;
		}

		public string[] Methods
		{
			get{ return _methods; }
		}

		public PathData GetPath(ContentItem item, string remainingUrl)
		{
			//if (string.IsNullOrEmpty(remainingUrl) || string.Equals(remainingUrl, "default", StringComparison.InvariantCultureIgnoreCase))
			//    remainingUrl = DefaultAction;
			int slashIndex = remainingUrl.IndexOf('/');
			
			string action = remainingUrl;
			string arguments = null;
			if(slashIndex > 0)
			{
				action = remainingUrl.Substring(0, slashIndex);
				arguments = remainingUrl.Substring(slashIndex + 1);
			}

			var templateUrl = GetTemplateUrl(item);
			var controllerName = _controllerMapper.GetControllerName(item.GetType());

			foreach(string method in _methods)
				if(method.Equals(action, StringComparison.InvariantCultureIgnoreCase))
					return new MvcPathData(item, templateUrl, action, arguments, controllerName);

			return null;
		}

		private string GetTemplateUrl(ContentItem item)
		{
			var templateUrl = String.Empty;
			var pathData = PathDictionary.GetFinders(item.GetType())
				.Where(finder => !(finder is ActionResolver))
				.Select(finder => finder.GetPath(item, null))
				.FirstOrDefault(path => path != null && !path.IsEmpty());

			if(pathData != null)
				templateUrl = pathData.TemplateUrl;
			return templateUrl;
		}
	}
}