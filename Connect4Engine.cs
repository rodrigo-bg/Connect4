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
        return false;
    }
    
    
}