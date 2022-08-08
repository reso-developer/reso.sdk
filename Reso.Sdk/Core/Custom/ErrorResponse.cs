using System;

namespace Reso.Sdk.Core.Custom
{
	public class ErrorResponse : Exception
	{
		public ErrorDetailResponse Error { get; private set; }

		public ErrorResponse(int errorCode, string message)
		{
			Error = new ErrorDetailResponse
			{
				Code = errorCode,
				Message = message
			};
		}

		public ErrorResponse()
		{
		}
	}
}
