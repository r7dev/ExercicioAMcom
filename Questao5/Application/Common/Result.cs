namespace Application.Common
{
	public class Result
	{
		public bool IsSuccess { get; private set; }
		public string ErrorCode { get; private set; }
		public string ErrorMessage { get; private set; }
		public object Value { get; private set; }

		private Result(bool isSuccess, string errorCode, string errorMessage, object value)
		{
			IsSuccess = isSuccess;
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
			Value = value;
		}

		public static Result Ok(object value = null)
		{
			return new Result(true, null, null, value);
		}

		public static Result Fail(string errorCode, string errorMessage)
		{
			return new Result(false, errorCode, errorMessage, null);
		}
	}
}
