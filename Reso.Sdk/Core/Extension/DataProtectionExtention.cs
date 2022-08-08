using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;

namespace Reso.Sdk.Core.Extension
{
	public static class DataProtectionExtention
	{
		public static void ConfigDataProtection(this IServiceCollection services)
		{
			services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
			{
				EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
				ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
			});
		}
	}
}
