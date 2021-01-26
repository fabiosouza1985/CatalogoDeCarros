import { Injectable } from '@angular/core';
import { Veiculo } from 'src/app/components/models/veiculo.model';
import { VeiculoFiltro } from 'src/app/components/models/veiculoFiltro.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CatalogoService {

    webApiURL: string = 'http://localhost:5000/api/veiculo/'

    constructor(private http: HttpClient) { }

    ObterVeiculos(filtro: VeiculoFiltro): Observable<Veiculo[]> {
        let httpParams = new HttpParams().set("Ano", filtro.Ano.toString()).set("Referencia", filtro.Refencia);
        return this.http.get<Veiculo[]>(this.webApiURL + "ObterVeiculos", { params: httpParams });
    };

    AdicionarVeiculo(veiculo: Veiculo): Observable<any> {
        return this.http.post<any>(this.webApiURL + 'Adicionar', veiculo);
    }

    AtualizarVeiculo(veiculo: Veiculo): Observable<any> {
        return this.http.post<any>(this.webApiURL + 'Atualizar', veiculo);
    }

    RemoverVeiculo(veiculo: Veiculo): Observable<any> {
        return this.http.post<any>(this.webApiURL + 'Remover', veiculo);
    }
}
