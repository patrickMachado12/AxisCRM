using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AxisCRM.Api.Domain.Services.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) {}
    }
}