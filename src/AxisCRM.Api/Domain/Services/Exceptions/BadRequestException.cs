namespace AxisCRM.Api.Domain.Services.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) {}
    }
}