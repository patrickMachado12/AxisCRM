using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Parecer;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IParecerService : IService<ParecerRequestDTO, ParecerResponseDTO, int>
    {
        
    }
}