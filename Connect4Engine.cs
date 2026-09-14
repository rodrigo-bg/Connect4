using Avalonia.Input;
using System.Collections.Generic;

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
            JuegoTerminado = true;
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

    public List<(int Fila, int Columna)> LineaGanadora { get; private set; } = new();

    public bool CompruebaVictoria(int fila, int columna)
    {
        Ficha ficha = Tablero[fila, columna];
        (int df, int dc)[] direcciones = { (0, 1), (1, 0), (1, 1), (-1, 1) };

        foreach (var (df, dc) in direcciones)
        {
            var linea = new List<(int, int)> { (fila, columna) };
            linea.AddRange(CeldasEnDireccion(fila, columna, df, dc, ficha));
            linea.AddRange(CeldasEnDireccion(fila, columna, -df, -dc, ficha));

            if (linea.Count >= 4)
            {
                LineaGanadora = linea;
                return true;
            }
        }
        return false;
    }

    private List<(int Fila, int Columna)> CeldasEnDireccion(int fila, int columna, int df, int dc, Ficha objetivo)
    {
        var celdas = new List<(int, int)>();
        int f = fila + df;
        int c = columna + dc;
        while (f >= 0 && f < Filas && c >= 0 && c < Columnas && Tablero[f, c] == objetivo)
        {
            celdas.Add((f, c));
            f += df;
            c += dc;
        }
        return celdas;
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