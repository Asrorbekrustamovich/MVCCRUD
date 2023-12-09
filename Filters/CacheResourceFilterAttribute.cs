using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Mime;

namespace MVCCRUD.Filters
{
    public class CacheResourceFilterAttribute : Attribute, IAsyncResourceFilter
    { private Dictionary<string, Stream>_cache = new Dictionary<string,Stream>();
        private string _cacheKey;
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
           _cacheKey=context.HttpContext.Request.Path.ToString();
            await next.Invoke();
            if(_cache.ContainsKey(_cacheKey))
            {


                context.HttpContext.Response.Body = _cache[_cacheKey];
                context.HttpContext.Request.Headers.Add("iscashed", "true");
                
               
            }
            await next.Invoke();
            if(string.IsNullOrEmpty(_cacheKey)&&!_cache.ContainsKey(_cacheKey)) 
            {
                
                _cache.Add(_cacheKey,context.HttpContext.Response.Body);
            }
        }
    }
}
