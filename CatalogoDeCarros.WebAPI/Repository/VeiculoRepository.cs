using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CatalogoDeCarros.WebAPI.Contexts;
using CatalogoDeCarros.WebAPI.Entities;
using CatalogoDeCarros.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogoDeCarros.WebAPI.Repository
{
    //Classe responsável por Obter, Adicionar, Atualizar e Remover Veículos
    public class VeiculoRepository
    {
        //Método que obtém veículos de acordo com a referência e ano
        public async Task<List<VeiculoModel>> ObterVeiculos(CatalogoContext catalogoContext, string Referencia, int Ano)
        {
         
            //Busca os dados de acordo com o filtro

            List<VeiculoModel> veiculoModels = await (from veiculo in catalogoContext.Veiculos
                                                      where veiculo.AnoModelo == Ano &&
                                                      veiculo.Referencia == Referencia
                                                      select new VeiculoModel
                                                      {
                                                          Referencia = veiculo.Referencia,
                                                          AnoModelo = veiculo.AnoModelo,
                                                          CodigoFipe = veiculo.CodigoFipe,
                                                          Marca = veiculo.Marca,
                                                          Modelo = veiculo.Modelo,
                                                          PrecoMedio = veiculo.PrecoMedio,
                                                          Codigo = veiculo.Codigo,
                                                          ValorRevenda = veiculo.ValorRevenda
                                                      }).ToListAsync();

            //Retorna uma lista de veículos
            return veiculoModels;

        }

        //Método responsável por adicionar um veículo
        public async Task<Object> Adicionar(CatalogoContext catalogoContext, VeiculoModel veiculoModel)
        {
            //Cria um novo veículo
            Veiculo veiculo = new Veiculo
            {
                ValorRevenda = veiculoModel.ValorRevenda,
                AnoModelo = veiculoModel.AnoModelo,
                CodigoFipe = veiculoModel.CodigoFipe,
                Marca = veiculoModel.Marca,
                Modelo = veiculoModel.Modelo,
                PrecoMedio = (decimal) veiculoModel.PrecoMedio,
                Referencia = veiculoModel.Referencia
            };

            try
            {
                //Marca o veículo como novo
                catalogoContext.Entry(veiculo).State = EntityState.Added;

                //Salva a inclusão
                await catalogoContext.SaveChangesAsync();

                //Recarrega o veículo para recuperar o Código
                await catalogoContext.Entry(veiculo).ReloadAsync();

                //Atualiza o Código do modelo
                veiculoModel.Codigo = veiculo.Codigo;

                //Retorna o Veículo atualizado
                return veiculoModel;
            }
            catch (DbUpdateException ex)
            {
                //Se houver erro, retorna exceção
                return ex.InnerException.ToString();

            }
        }

        //Método responsável por atualizar um veículo
        public async Task<Object> Atualizar(CatalogoContext catalogoContext, VeiculoModel veiculoModel)
        {
            //Busca o veículo no banco de dados
            Veiculo veiculo = await catalogoContext.Veiculos.FindAsync(veiculoModel.Codigo);

            //Se não encontra o veículo, retorna erro
            if(veiculo == null)
            {
                return "Veículo não encontrado";
            }

            //Atualiza os dados do veículo
            veiculo.CodigoFipe = veiculoModel.CodigoFipe;
            veiculo.AnoModelo = veiculoModel.AnoModelo;
            veiculo.Marca = veiculoModel.Marca;
            veiculo.Modelo = veiculoModel.Modelo;
            veiculo.PrecoMedio = veiculoModel.PrecoMedio;
            veiculo.Referencia = veiculoModel.Referencia;
            veiculo.ValorRevenda = veiculoModel.ValorRevenda;

            try
            {
                //Marca o veículo como modificado
                catalogoContext.Entry(veiculo).State = EntityState.Modified;

                //Salva as alterações
                await catalogoContext.SaveChangesAsync();             

                //Retorna o veículo atualizado
                return veiculoModel;
            }
            catch (DbUpdateException ex)
            {
                //Se houver erro, retorna exceção
                return ex.InnerException.ToString();

            }
        }

        //Método responsável por remover um veículo
        public async Task<Object> Remover(CatalogoContext catalogoContext, VeiculoModel veiculoModel)
        {
            //Busca o veículo no banco de dados
            Veiculo veiculo = await catalogoContext.Veiculos.FindAsync(veiculoModel.Codigo);

            //Se não encontra o veículo, retorna erro
            if (veiculo == null)
            {
                return "Veículo não encontrado";
            }

            try
            {
                //Marca o veículo como deletado
                catalogoContext.Entry(veiculo).State = EntityState.Deleted;

                //Salva as alterações
                await catalogoContext.SaveChangesAsync();

                //Retorna o veículo deletado
                return veiculoModel;
            }
            catch (DbUpdateException ex)
            {
                //Se houver erro, retorna exceção
                return ex.InnerException.ToString();

            }
        }
    }
}
