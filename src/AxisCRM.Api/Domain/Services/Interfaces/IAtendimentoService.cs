using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.DTO.Atendimento;

namespace AxisCRM.Api.Domain.Services.Interfaces
{
    public interface IAtendimentoService : IService<AtendimentoRequestDTO, AtendimentoResponseDTO, int>
    {
        
    }
}