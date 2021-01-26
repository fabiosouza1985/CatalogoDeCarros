using CatalogoDeCarros.WebAPI.Contexts;
using CatalogoDeCarros.WebAPI.Models;
using CatalogoDeCarros.WebAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogoDeCarros.WebAPI.Controllers
{
  
    //Controle responsável por fazer chamadas afim de Obter, Adicionar, Atualizar e Remover Veículos
    //Métodos são chamados através de requests, informado a parte do caminho mencionada na tag Route() mais o nome do método
    [ApiController]
    [Route("api/veiculo")]
    public class VeiculoController : ControllerBase
    {
              
        //Função responsável por Obter os Veículos de Acordo com a Referência e Ano do Modelo
        [HttpGet]
        [Route("ObterVeiculos")]
        public async Task<List<VeiculoModel>> ObterVeiculos([FromServices] CatalogoContext catalogoContext, string Referencia, int Ano)
        {
            VeiculoRepository veiculoRepository = new VeiculoRepository();
            //Retorna lista de veículos
            return await veiculoRepository.ObterVeiculos(catalogoContext, Referencia, Ano);
        }

        //Função Responsável por Adicionar um Novo Veículo
        [HttpPost]
        [Route("Adicionar")]
        public async Task<ActionResult> Adicionar([FromServices] CatalogoContext catalogoContext, [FromBody] VeiculoModel veiculoModel)
        {
            //Faz a validação do Veículo. Caso possua algum dado inválido, retorna erro.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VeiculoRepository veiculoRepository = new VeiculoRepository();

          
            object result = await veiculoRepository.Adicionar(catalogoContext, veiculoModel);

           
            if (result.GetType() == typeof(VeiculoModel))
            {
                return Ok(result);
            }
            else
            {
                //Se não retornar um objeto do tipo VeiculoModel, retorna erro.
                ModelState.AddModelError("errors", result.ToString());
                return BadRequest(ModelState);
            }
        }

        //Método responsável por atualizar um veículo
        [HttpPost]
        [Route("Atualizar")]
        public async Task<ActionResult> Atualizar([FromServices] CatalogoContext catalogoContext, [FromBody] VeiculoModel veiculoModel)
        {
            //Faz a validação do Veículo. Caso possua algum dado inválido, retorna erro.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VeiculoRepository veiculoRepository = new VeiculoRepository();
            object result = await veiculoRepository.Atualizar(catalogoContext, veiculoModel);

            if (result.GetType() == typeof(VeiculoModel))
            {
                return Ok(result);
            }
            else
            {
                //Se não retornar um objeto do tipo VeiculoModel, retorna erro.
                ModelState.AddModelError("errors", result.ToString());
                return BadRequest(ModelState);
            }
        }

        //Método responsável por remover um veículo
        [HttpPost]
        [Route("Remover")]
        public async Task<ActionResult> Remover([FromServices] CatalogoContext catalogoContext, [FromBody] VeiculoModel veiculoModel)
        {
          
            VeiculoRepository veiculoRepository = new VeiculoRepository();
            object result = await veiculoRepository.Remover(catalogoContext, veiculoModel);

            if (result.GetType() == typeof(VeiculoModel))
            {
                return Ok(result);
            }
            else
            {
                //Se não retornar um objeto do tipo VeiculoModel, retorna erro.
                ModelState.AddModelError("errors", result.ToString());
                return BadRequest(ModelState);
            }
        }
    }
}
