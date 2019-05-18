namespace Timereporter.Api.Controllers
{
	public enum TransactionResult
	{
		Ok = 200,
		NotFound = 404,
		Conflict = 409,
		NotModified = 304
	}
}
