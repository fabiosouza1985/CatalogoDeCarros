using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CatalogoDeCarros.WebAPI.Models
{
    //Classe Cópia da Tabela Veículo
    //Utilizado para não trabalhar diretamente com o objeto retornado do banco de dados
    public class VeiculoModel
    {
        //Mensagem de erro para campos obrigatórios
        private const string RequiredMessage = "O campo {0} é obrigatório";

        //Mensagem de erro para inserção de caracteres além do limite permitido
        private const string StringLengthMessage = "O campo {0} deve ter, no máximo, {1} caracteres";


        //Código do Veículo
        [Required(ErrorMessage = RequiredMessage)]
        public int Codigo { get; set; }


        //Marca do Veículo
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(100, ErrorMessage = StringLengthMessage)]
        public string Marca { get; set; }


        //Modelo do Veículo
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(100, ErrorMessage = StringLengthMessage)]
        public string Modelo { get; set; }

        //Ano do Modelo do Veículo
        [Required(ErrorMessage = RequiredMessage)]
        public Int16 AnoModelo { get; set; }

        //Codigo da Tabela FIPE do veículo
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(8, ErrorMessage = StringLengthMessage)]
        public string CodigoFipe { get; set; }

        //Preço Médio do Veículo
        [Required(ErrorMessage = RequiredMessage)]
        public decimal PrecoMedio { get; set; }

        //Referência da Tabela FIPE
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(20, ErrorMessage = StringLengthMessage)]
        public string Referencia { get; set; }

        //Valor de Revenda
        public decimal? ValorRevenda { get; set; }
    }
}
