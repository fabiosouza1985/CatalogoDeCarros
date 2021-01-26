export interface Veiculo {
    codigo: number,
    marca: string,
    modelo: string,
    anoModelo: number,
    codigoFipe: string,
    precoMedio: number,
    referencia: string,
    valorRevenda?: number
}