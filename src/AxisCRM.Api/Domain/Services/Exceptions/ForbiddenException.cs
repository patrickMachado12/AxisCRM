namespace AxisCRM.Api.Domain.Services.Exceptions
{
    public class ForbiddenException :Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
}