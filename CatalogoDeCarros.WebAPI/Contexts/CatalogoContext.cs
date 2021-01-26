using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogoDeCarros.WebAPI.Entities;
using System.Configuration;

namespace CatalogoDeCarros.WebAPI.Contexts
{
    //Classe responsável pela conexão com o banco de dados
    public class CatalogoContext : DbContext
    {
       
        //Responsável por retornar e manter os dados da tabela Veículos
        public DbSet<Veiculo> Veiculos { get; set; }

        public CatalogoContext(DbContextOptions<CatalogoContext> options)
            : base(options)
        { }

       
    }
}
