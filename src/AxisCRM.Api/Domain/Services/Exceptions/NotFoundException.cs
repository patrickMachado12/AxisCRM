namespace AxisCRM.Api.Domain.Services.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) {}
    }
}