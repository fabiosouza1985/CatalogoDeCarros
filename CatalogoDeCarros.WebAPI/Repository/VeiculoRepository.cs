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
using Newtonsoft.Json.Linq;
using System.Net.Http.Formatting;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace CatalogoDeCarros.WebAPI.Repository
{
    //Classe responsável por Obter, Adicionar, Atualizar e Remover Veículos
    public class VeiculoRepository
    {

        //Método responsável por buscar os dados do veículo direto do site do governo
        private async Task<List<VeiculoModel>> ObterVeiculosTabelaFipe(CatalogoContext catalogoContext, string Referencia, int Ano)
        {                   

            //Configuração de proxy
            //Remover ou configurar de acordo com a necessidade
            HttpClient httpClient = new HttpClient(new HttpClientHandler()
            {
                UseProxy = true,
                Proxy = null, // use system proxy
                DefaultProxyCredentials = new System.Net.NetworkCredential("fabio.souza",    "fs123456")
            });

            //Retorna as tabelas de referência
            var resposta = await httpClient.PostAsync("https://veiculos.fipe.org.br/api/veiculos//ConsultarTabelaDeReferencia", null);

            //Cria uma lista de veículos
            List<VeiculoModel> lista = new List<VeiculoModel>();
            
            //Se o retorno é bem sucedido, prossegue
            if (resposta.IsSuccessStatusCode)
            {
                //Lê o conteúdo da resposta
                List<JObject> conteudo = await resposta.Content.ReadAsAsync<List<JObject>>();

                //Retorna o código da tabela de acordo com o ano informado
                var codigo_tabela = conteudo.Where(e => e.GetValue("Mes").ToString().Replace(" ", "") == Referencia).Select(e => e.GetValue("Codigo").ToString()).FirstOrDefault();

                //Se o código não é nulo, prossegue
                if (codigo_tabela != null)
                {
                    //Cria os parâmetros para a próxima requisição
                    var pairs = new List<KeyValuePair<string, string>>();
                    pairs.Add(new KeyValuePair<string, string>("codigoTabelaReferencia", codigo_tabela));
                    pairs.Add(new KeyValuePair<string, string>("codigoTipoVeiculo", "1"));

                    //cria o corpo da requisição
                    var body = new FormUrlEncodedContent(pairs);

                    //Retorna as marcas de acordo com a tabela de referência
                    resposta = await httpClient.PostAsync("https://veiculos.fipe.org.br/api/veiculos//ConsultarMarcas", body);                    
                    var marcas = await resposta.Content.ReadAsAsync<List<JObject>>();

                    //Percorre as marcas
                    for(var i = 0; i < marcas.Count; i++)
                    {
                        //Cria os parâmetros para a próxima requisição
                        pairs = new List<KeyValuePair<string, string>>();
                        pairs.Add(new KeyValuePair<string, string>("codigoTabelaReferencia", codigo_tabela));
                        pairs.Add(new KeyValuePair<string, string>("codigoTipoVeiculo", "1"));
                        pairs.Add(new KeyValuePair<string, string>("codigoMarca", marcas[i].GetValue("Value").ToString()));

                        //cria o corpo da requisição
                        body = new FormUrlEncodedContent(pairs);

                        //Retorna os modelos de acordo com a tabela de referência e marca
                        resposta = await httpClient.PostAsync("https://veiculos.fipe.org.br/api/veiculos//ConsultarModelos", body);
                        JObject Ano_Modelos = await resposta.Content.ReadAsAsync<JObject>();
                        List<JObject> Modelos = Ano_Modelos.GetValue("Modelos").ToObject<List<JObject>>();
                      
                       
                        //Percorre os modelos
                       for(var m =0; m < Modelos.Count; m++)
                        {

                            //Cria os parâmetros para a próxima requisição
                            pairs = new List<KeyValuePair<string, string>>();
                            pairs.Add(new KeyValuePair<string, string>("codigoTabelaReferencia", codigo_tabela));
                            pairs.Add(new KeyValuePair<string, string>("codigoTipoVeiculo", "1"));
                            pairs.Add(new KeyValuePair<string, string>("codigoMarca", marcas[i].GetValue("Value").ToString()));
                            pairs.Add(new KeyValuePair<string, string>("codigoModelo", Modelos[m].GetValue("Value").ToString()));                            
                            pairs.Add(new KeyValuePair<string, string>("anoModelo", Ano.ToString()));
                            pairs.Add(new KeyValuePair<string, string>("codigoTipoVeiculo", "1"));
                            pairs.Add(new KeyValuePair<string, string>("codigoTipoCombustivel", "1"));
                            pairs.Add(new KeyValuePair<string, string>("tipoConsulta", "tradicional"));
                            pairs.Add(new KeyValuePair<string, string>("tipoVeiculo", "carro"));

                            //cria o corpo da requisição
                            body = new FormUrlEncodedContent(pairs);

                            //Retorna os dados do veículo de acordo com a tabela de referência, marca, modelo e ano do modelo
                            resposta = await httpClient.PostAsync("https://veiculos.fipe.org.br/api/veiculos//ConsultarValorComTodosParametros", body);
                            JObject Veiculo = await resposta.Content.ReadAsAsync<JObject>();

                            //Se o veículo é encontrado e possui valor, prossegue
                            if(Veiculo != null)
                            {
                                if (Veiculo.Property("Valor") != null)
                                {

                                
                                    var style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
                                    var provider = new CultureInfo("pt-BR");

                                    string valor = Veiculo.GetValue("Valor").ToString();
                                    valor = valor.Replace("R$ ", "").Replace(".", "");

                                    VeiculoModel veiculoModel = new VeiculoModel()
                                    {

                                        AnoModelo = short.Parse(Veiculo.GetValue("AnoModelo").ToString()),
                                        CodigoFipe = Veiculo.GetValue("CodigoFipe").ToString(),
                                        Marca = Veiculo.GetValue("Marca").ToString(),
                                        Modelo = Veiculo.GetValue("Modelo").ToString(),
                                        PrecoMedio = Decimal.Parse(valor, style, provider),
                                        Referencia = Referencia
                                    };

                                    //Adiciona o veículo no banco de dados
                                    var result = await Adicionar(catalogoContext, veiculoModel);

                                    //Se é adicionado com sucesso, adiciona na lista
                                    if(result.GetType() == typeof(VeiculoModel))
                                    {
                                        veiculoModel = (VeiculoModel)await Adicionar(catalogoContext, veiculoModel);

                                        lista.Add(veiculoModel);
                                    }
                                  

                                    //Limitado a 10 pois o processo leva muito tempo
                                    if (lista.Count >= 10)
                                    {
                                        return lista;
                                    }
                                }
                              
                                
                            }
                            

                        }
                        

                    }
                  
                }
              
            }           

            //Retorna a lista
            return lista;
            
        } 


        //Método que obtém veículos de acordo com a referência e ano
        public async Task<List<VeiculoModel>> ObterVeiculos(CatalogoContext catalogoContext, string Referencia, int Ano)
        {
         
            //Busca os dados de acordo com o filtro

            List<VeiculoModel> veiculosModels = await (from veiculo in catalogoContext.Veiculos
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

            //Se nenhum veículo é encontrado, retorna os veículos da tabela FIPE
            if(veiculosModels.Count == 0)
            {
                veiculosModels = await ObterVeiculosTabelaFipe(catalogoContext, Referencia, Ano);
            }

            //Retorna uma lista de veículos
            return veiculosModels;

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
