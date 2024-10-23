using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract.IResponseCacheService;

namespace Talabat.APIs.Helpers
{
	public class CashedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveInSeconds;

		public CashedAttribute(int timeToLiveInSeconds)
        {
			_timeToLiveInSeconds = timeToLiveInSeconds;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			// Ask CLR For Creating Object From "ResponseCacheService" Explicitly
 			var responseCacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();	

			var cacheKey = GenerateCacheKeyFromRequst(context.HttpContext.Request);

			var response = await responseCacheService.GetCachedResponseAsync(cacheKey);

			if (!string.IsNullOrEmpty(response))
			{
				var result = new ContentResult()
				{
					Content = response,	
					ContentType = "application/json",
					StatusCode = 200,
				};

				context.Result = result;
				return;
			}

		    var excutedActionContext = await next.Invoke();         // In Case response is Not Cached
			if(excutedActionContext.Result is OkObjectResult okObjectResult && okObjectResult.Value is not null)
			{
				await responseCacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveInSeconds));
			}
		}

		private string GenerateCacheKeyFromRequst(HttpRequest request)
		{
			// {{ur1}}/api/products?pageIndex=1&pageSize=5&sort=name
			var keyBuilder = new StringBuilder();

			keyBuilder.Append(request.Path);  //api/products

			foreach(var (key, value) in request.Query.OrderBy(x => x.Key))
			{
				keyBuilder.Append($"| {key}-{value}");
			}

			return keyBuilder.ToString();
		}
	}
}
