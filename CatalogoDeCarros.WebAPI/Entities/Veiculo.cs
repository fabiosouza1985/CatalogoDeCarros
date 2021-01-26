using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogoDeCarros.WebAPI.Entities
{

    //Classe que representa o objeto Veículo no banco de dados
    [Table("Veiculos")]
    public class Veiculo
    {
        //Mensagem de erro para campos obrigatórios
        private const string RequiredMessage = "O campo {0} é obrigatório";

        //Mensagem de erro para inserção de caracteres além do limite permitido
        private const string StringLengthMessage = "O campo {0} deve ter, no máximo, {1} caracteres";

        //Código do Veículo
        //Chave primário, auto numeração
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Column(TypeName = "decimal(10,2)")]
        [Required(ErrorMessage = RequiredMessage)]
        public decimal PrecoMedio { get; set; }

        //Referência da Tabela FIPE
        [Required(ErrorMessage = RequiredMessage)]
        [StringLength(20, ErrorMessage = StringLengthMessage)]
        public string Referencia { get; set; }

        //Valor de Revenda
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorRevenda { get; set; }
    }
}
