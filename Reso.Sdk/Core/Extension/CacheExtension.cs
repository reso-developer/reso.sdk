using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Reso.Sdk.Core.Extension
{
	public static class CacheExtension
	{
		public static async Task SetObjectAsync<T>(this IDistributedCache distributedCache, string key, T value, CancellationToken token = default(CancellationToken))
		{
			await distributedCache.SetAsync(key, value.ToByteArray(), token);
		}

		public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken token = default(CancellationToken)) where T : class
		{
			return (await distributedCache.GetAsync(key, token)).FromByteArray<T>();
		}

		public static byte[] ToByteArray(this object obj)
		{
			if (obj == null)
			{
				return null;
			}
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
		}

		public static T FromByteArray<T>(this byte[] byteArray) where T : class
		{
			if (byteArray == null)
			{
				return null;
			}
			return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(byteArray));
		}

		public static void ConfigMemoryCacheAndRedisCache(this IServiceCollection services, string configuration)
		{
			services.AddMemoryCache();
			services.AddStackExchangeRedisCache(delegate(RedisCacheOptions option)
			{
				option.Configuration = configuration;
			});
		}
	}
}
