using Avalonia.Input;

namespace Connect4;

public enum Ficha {Vacio = 0, Rojo = 1, Amarillo = 2}

public class Connect4Engine
{
    public const int Filas = 6;
    public const int Columnas = 7;

    public Ficha[,] Tablero { get; } = new Ficha[Filas, Columnas];
    public Ficha TurnoActual { get; private set; } = Ficha.Rojo;
    public bool JuegoTerminado { get; private set; } = false;
    public Ficha Ganador { get; private set; } = Ficha.Vacio;

// Busca la primera fila libre de abajo hacia arriba
    public int BuscaFilaLibre(int columna) {
        if (columna < 0 || columna > Columnas) return -1;
        for (int f = Filas - 1; f >= 0; f--)
        {
            if (Tablero[f, columna] == Ficha.Vacio)
                return f;
        }
        return -1;
    }

    public int SueltaFicha(int columna)
    {
        if (JuegoTerminado) return -1;
        int fila = BuscaFilaLibre(columna);
        if (fila == -1) return -1;

        Tablero[fila, columna] = TurnoActual;
        if (CompruebaVictoria(fila, columna))
        {
            JuegoTerminado = true;
            Ganador = TurnoActual;
        } 
        else if (TableroEstaLleno())
        {
            JuegoTerminado = false;
            Ganador = Ficha.Vacio;
        }
        else
        {
            CambiaTurno();
        }
        return fila;
    }

    private void CambiaTurno()
    {
        TurnoActual = (TurnoActual == Ficha.Rojo) ? Ficha.Amarillo : Ficha.Rojo;
    }

    private bool TableroEstaLleno()
    {
        for (int c = 0; c < Columnas; c++)
        {
            if (Tablero[0, c] == Ficha.Vacio) return false;
        }
        return true;
    }

    public bool CompruebaVictoria(int fila, int columna)
    {
        Ficha ficha = Tablero[fila, columna];
        (int df, int dc)[] direcciones = { (0, 1), (1, 0), (1, 1), (-1, 1) };

        foreach (var (df, dc) in direcciones)
        {
            int total = 1;
            // cuenta hacia adelante
            total += CuentaEnDireccion(fila, columna, df, dc, ficha);
            
            //cuenta hacia atras invirtiendo el vector
            total += CuentaEnDireccion(fila, columna, -df, -dc, ficha);
            
            if (total >= 4) return true;
        }
        return false;
    }

    public int CuentaEnDireccion(int fila, int columna, int df, int dc, Ficha objetivo)
    { 
        int cuenta = 0;
        int f = fila + df;
        int c = columna + dc;
        while (f >= 0 && f < Filas && c >= 0 && c < Columnas && Tablero[f, c] == objetivo)
        {
            cuenta++;
            f += df;
            c += dc;
        }
        return cuenta;
    }

    public void Reincia()
    {
        for (int f = 0; f < Filas; f++)
            for (int c =  0; c < Columnas; c++)
                Tablero[f, c] = Ficha.Vacio;

        TurnoActual = Ficha.Rojo;
        JuegoTerminado = false;
        Ganador = Ficha.Vacio;
    }
    
}