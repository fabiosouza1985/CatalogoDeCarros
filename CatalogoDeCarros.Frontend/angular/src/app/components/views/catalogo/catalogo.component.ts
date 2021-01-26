import { Component, OnInit } from '@angular/core';
import { Veiculo } from 'src/app/components/models/veiculo.model';
import { VeiculoFiltro } from 'src/app/components/models/veiculoFiltro.model';
import { CatalogoService } from 'src/app/components/services/catalogo.service';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-catalogo',
  templateUrl: './catalogo.component.html',
  styleUrls: ['./catalogo.component.css']
})
export class CatalogoComponent implements OnInit {

    anos: number[] = [];
    referencias: string[] = [];
    ano_selecionado: number = null;
    referencia_selecionada: string = null;
    veiculos: Veiculo[] = [];
    custo: number;
    carregando: boolean = false;
    datasource = new MatTableDataSource();

    constructor(private catalogoService: CatalogoService) { }

    ngOnInit(): void {
       
      
        for (var ano = 1995; ano <= 2020; ano++) {
            this.anos.splice(0, 0,ano);
        }

        let ano_atual = new Date().getFullYear();
        let mes_atual = new Date().getMonth() + 1;

        for (var ano = 2001; ano <= ano_atual; ano++) {

            for (var mes = 1; mes <= 12; mes++) {

                if (ano == ano_atual && mes > mes_atual) {
                    break;
                } else {
                    this.referencias.splice(0, 0, this.ConverterMes(mes) + '/' + ano);
                }


            }

        }
    }

    ConverterMes(mes: number): string {
        switch (mes) {
            case 1:
                return 'janeiro'
            case 2:
                return 'fevereiro'
            case 3:
                return 'março'
            case 4:
                return 'abril'
            case 5:
                return 'maio'
            case 6:
                return 'junho'
            case 7:
                return 'julho'
            case 8:
                return 'agosto'
            case 9:
                return 'setembro'
            case 10:
                return 'outubro'
            case 11:
                return 'novembro'
            case 12:
                return 'dezembro'

        }
    }

    ObterVeiculos() {
        if (this.ano_selecionado == null || this.referencia_selecionada == null) {
            return;
        }
        this.carregando = true;

        this.veiculos = [];
        this.datasource = new MatTableDataSource();

        let VeiculoFiltro: VeiculoFiltro = {
            Ano: this.ano_selecionado,
            Refencia: this.referencia_selecionada
        };
       
        this.catalogoService.ObterVeiculos(VeiculoFiltro).subscribe(results => {
            this.veiculos = results;   
            this.veiculos.splice(0, 0, { codigo: 0, anoModelo: this.ano_selecionado, codigoFipe: '', marca: '', modelo: '', precoMedio: null, referencia: this.referencia_selecionada, valorRevenda: null });

            this.datasource = new MatTableDataSource(this.veiculos);
            this.carregando = false;
        })
    }

    AdicionarVeiculo(veiculo: Veiculo): void {

        veiculo.referencia = this.referencia_selecionada;
            
        this.catalogoService.AdicionarVeiculo(veiculo).subscribe(results => {

            veiculo.codigo = 0;
            veiculo.anoModelo = this.ano_selecionado;
            veiculo.codigoFipe = '';
            veiculo.marca = '';
            veiculo.modelo = '';
            veiculo.precoMedio = null;
            veiculo.referencia = this.referencia_selecionada;
            veiculo.valorRevenda = null;

            this.veiculos.push(results);
            this.datasource.data = this.veiculos;
            alert("Veículo adicionado com sucesso.");
        },
            (err) => {
               
                if (err.error.errors !== undefined) {
                    let properties = Object.getOwnPropertyNames(err.error.errors);

                    var erros = '';
                    for (var e = 0; e < properties.length; e++) {
                        if (properties[e] !== 'length') {
                            erros += '- ' + err.error.errors[properties[e]] + '\n';
                        };
                    }
                    alert(erros);
                }
                console.log(err)
            })
    };

    AtualizarVeiculo(veiculo: Veiculo): void {
      
        this.catalogoService.AtualizarVeiculo(veiculo).subscribe(results => {
         
            alert('Veículo atualizado');
        },
            (err) => {
            
                if (err.error.errors !== undefined) {
                    let properties = Object.getOwnPropertyNames(err.error.errors);

                    var erros = '';
                    for (var e = 0; e < properties.length; e++) {
                        if (properties[e] !== 'length') {
                            erros += '- ' + err.error.errors[properties[e]] + '\n';
                        };
                    }
                    alert(erros);
                }

                console.log(err)
            })
    };

    RemoverVeiculo(veiculo: Veiculo): void {
       
        this.catalogoService.RemoverVeiculo(veiculo).subscribe(results => {
            this.veiculos.splice(this.veiculos.indexOf(veiculo), 1);
            this.datasource.data = this.veiculos;
            alert("Veículo removido com sucesso");
        },
            (err) => {
              
                if (err.error.errors !== undefined) {
                    let properties = Object.getOwnPropertyNames(err.error.errors);

                    var erros = '';
                    for (var e = 0; e < properties.length; e++) {
                        if (properties[e] !== 'length') {
                            erros += '- ' + err.error.errors[properties[e]] + '\n';
                        };
                    }
                    alert(erros);
                }
                console.log(err)
            })

    };

    CalcularValorRevenda() {
        if (this.veiculos.length == 0) {
            return;
        }

        for (var i = 1; i < this.veiculos.length; i++) {
            var custo = this.veiculos[i].precoMedio * this.custo / 100;
            this.veiculos[i].valorRevenda = this.veiculos[i].precoMedio + custo;
            this.AtualizarVeiculo(this.veiculos[i]);
        }
    }
}
