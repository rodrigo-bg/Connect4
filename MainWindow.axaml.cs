using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia;

namespace Connect4;

public partial class MainWindow : Window
{
    // Dimensiones en constantes: cambias esto y el tablero entero cambia.
    private const int Filas = 6;
    private const int Columnas = 7;

    // Referencia a cada círculo, para poder pintarlo después.
    private readonly Ellipse[,] _celdas = new Ellipse[Filas, Columnas];

    // Columna donde está parado el selector (arranca al centro).
    private int _columnaActual = 3;
    
    // Valores posibles de una casilla. Constantes, no números mágicos.
    private const int Vacio    = 0;
    private const int Rojo     = 1;
    private const int Amarillo = 2;

    // EL TABLERO DE VERDAD. int[,] arranca lleno de 0 = Vacio, gratis.
    private readonly int[,] _tablero = new int[Filas, Columnas];

    // De quién es el turno.
    private int _turno = Rojo;

    // COLORES DE FICHAS
    private static readonly IBrush ColorVacio    = new SolidColorBrush(Color.Parse("#12122E"));
    private static readonly IBrush ColorSelector = Brushes.White;
    
    private static readonly IBrush BordeVacio    = new SolidColorBrush(Color.Parse("#2A2A55"));
    private static readonly IBrush ColorRojo     = new SolidColorBrush(Color.Parse("#E74C3C"));
    private static readonly IBrush BordeRojo     = new SolidColorBrush(Color.Parse("#96281B"));
    private static readonly IBrush ColorAmarillo = new SolidColorBrush(Color.Parse("#F1C40F"));
    private static readonly IBrush BordeAmarillo = new SolidColorBrush(Color.Parse("#B7950B"));
    
    public MainWindow()
    {
        InitializeComponent();
        ConstruirTablero();
        RedibujarTablero();
    }

    // Crea los 42 círculos y los mete al UniformGrid.
    private void ConstruirTablero()
    {
        for (int f = 0; f < Filas; f++)
        {
            for (int c = 0; c < Columnas; c++)
            {
                var celda = new Ellipse
                {
                    Width = 48,
                    Height = 48,
                    Margin = new Thickness(4),
                    Fill = ColorVacio,
                    Stroke = BordeVacio,
                    StrokeThickness = 2
                };

                _celdas[f, c] = celda;      // la guardo para tocarla luego
                TableroGrid.Children.Add(celda); // y la muestro
            }
        }
    }

    private void ActualizarTurno()
    {
        if (_turno == Rojo)
        {
            TurnoText.Text = "TURNO: ROJO";
            TurnoText.Foreground = ColorRojo;
        }
        else
        {
            TurnoText.Text = "TURNO: AMARILLO";
            TurnoText.Foreground = ColorAmarillo;
        }
    }

    private void ActualizarBotones()
    {
        // SOLTAR se apaga si la columna ya está llena.
        BtnSoltar.IsEnabled = BuscarFilaLibre(_columnaActual) != -1;
    }
    
    
    // Repinta todo: vacío por defecto, blanco en la columna seleccionada.
    
    private void RedibujarTablero()
    {
        for (int f = 0; f < Filas; f++)
        {
            for (int c = 0; c < Columnas; c++)
            {
                int valor = _tablero[f, c];
                bool seleccionada = (c == _columnaActual);
                if (valor == Rojo)
                {
                    _celdas[f, c].Fill   = ColorRojo;
                    _celdas[f, c].Stroke = BordeRojo;
                }
                else if (valor == Amarillo)
                {
                    _celdas[f, c].Fill   = ColorAmarillo;
                    _celdas[f, c].Stroke = BordeAmarillo;
                }
                else if (seleccionada)
                {
                    _celdas[f, c].Fill   = ColorSelector;
                    _celdas[f, c].Stroke = ColorSelector;
                }
                else
                {
                    _celdas[f, c].Fill   = ColorVacio;
                    _celdas[f, c].Stroke = BordeVacio;
                }
            }
        }

        ActualizarTurno();
        ActualizarBotones();
    }

    private void MoverSelector(int delta)
    {
        int nueva = _columnaActual + delta;
        if(nueva >= Columnas)
        {
            nueva = nueva % Columnas;
        }
        else if (nueva < 0)
        {
            nueva = Columnas-1;
        }
        _columnaActual = nueva;
        RedibujarTablero();
    }

    private void BtnIzq_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => MoverSelector(-1);

    private void BtnDer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => MoverSelector(+1);

    private void BtnSoltar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 1. ¿Dónde cae?
        int fila = BuscarFilaLibre(_columnaActual);

        // 2. Guard clause: columna llena, no hago nada.
        if (fila == -1) return;

        // 3. Modifico el ESTADO (no la pantalla).
        _tablero[fila, _columnaActual] = _turno;

        // 4. Cambio el turno.
        _turno = (_turno == Rojo) ? Amarillo : Rojo;

        // 5. Ahora sí, que la pantalla se ponga al día.
        RedibujarTablero();
    }
    
    /// Devuelve la fila donde caería una ficha en esa columna.
    /// Devuelve -1 si la columna ya está llena.
    private int BuscarFilaLibre(int columna)
    {
        for (int f = Filas - 1; f >= 0; f--)
        {
            if (_tablero[f, columna] == Vacio)
                return f;
        }
        return -1;
    }
    
    

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)  MoverSelector(-1);
        if (e.Key == Key.Right) MoverSelector(+1);
        if (e.Key == Key.Space || e.Key == Key.Down) BtnSoltar_Click(sender, new());
    }
    
    
}
