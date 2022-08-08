using System;
using System.Threading.Tasks;

namespace Reso.Sdk.Core.BaseConnect
{
	public interface IUnitOfWork : IDisposable
	{
		int Commit();

		Task<int> CommitAsync();
	}
}
