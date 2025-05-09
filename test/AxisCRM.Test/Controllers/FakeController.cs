using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AxisCRM.Test.Controllers
{
    public class FakeController : BaseController
    {
        public Task<ActionResult<T>> ExecutarProcessarTarefa<T>(Task<T> tarefa, bool isNovoCadastro = false)
        {
            return ProcessarTarefa(tarefa, isNovoCadastro);
        }
    }
}