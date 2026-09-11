using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Controls.Shapes;
using Avalonia;

namespace Connect4;

public partial class MainWindow : Window
{
    private const int Filas = 6;
    private const int Columnas = 7;

    private readonly Ellipse[,] _celdas = new Ellipse[Filas, Columnas];
    private int _columnaActual = 3;

    private readonly Connect4Engine _engine = new();

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

    private void ConstruirTablero()
    {
        for (int f = 0; f < Filas; f++)
            for (int c = 0; c < Columnas; c++)
            {
                var celda = new Ellipse
                {
                    Width = 48, Height = 48, Margin = new Thickness(4),
                    Fill = ColorVacio, Stroke = BordeVacio, StrokeThickness = 2
                };
                _celdas[f, c] = celda;
                TableroGrid.Children.Add(celda);
            }
    }

    private void ActualizarTurno()
    {
        if (_engine.JuegoTerminado)
        {
            TableroBorder.IsVisible = false;   // se borra el tablero
            PantallaGanador.IsVisible = true;  // aparece GANASTE + REINICIAR
            TurnoText.Text = "";

            GanadorText.Foreground = _engine.Ganador == Ficha.Rojo ? ColorRojo : ColorAmarillo;
            return;
        }

        TableroBorder.IsVisible = true;
        PantallaGanador.IsVisible = false;

        if (_engine.TurnoActual == Ficha.Rojo)
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
        bool sePuedeJugar = !_engine.JuegoTerminado;
        BtnSoltar.IsEnabled = sePuedeJugar && _engine.BuscaFilaLibre(_columnaActual) != -1;
        BtnIzq.IsEnabled = sePuedeJugar;
        BtnDer.IsEnabled = sePuedeJugar;
    }

    private void RedibujarTablero()
    {
        for (int f = 0; f < Filas; f++)
        {
            for (int c = 0; c < Columnas; c++)
            {
                Ficha valor = _engine.Tablero[f, c];
                bool seleccionada = c == _columnaActual && !_engine.JuegoTerminado;

                if (valor == Ficha.Rojo)
                {
                    _celdas[f, c].Fill = ColorRojo;
                    _celdas[f, c].Stroke = BordeRojo;
                }
                else if (valor == Ficha.Amarillo)
                {
                    _celdas[f, c].Fill = ColorAmarillo;
                    _celdas[f, c].Stroke = BordeAmarillo;
                }
                else if (seleccionada)
                {
                    _celdas[f, c].Fill = ColorSelector;
                    _celdas[f, c].Stroke = ColorSelector;
                }
                else
                {
                    _celdas[f, c].Fill = ColorVacio;
                    _celdas[f, c].Stroke = BordeVacio;
                }
            }
        }

        ActualizarTurno();
        ActualizarBotones();
    }

    private void MoverSelector(int delta)
    {
        if (_engine.JuegoTerminado) return;
        int nueva = _columnaActual + delta;
        if (nueva >= Columnas) nueva = nueva % Columnas;
        else if (nueva < 0) nueva = Columnas - 1;
        _columnaActual = nueva;
        RedibujarTablero();
    }

    private void BtnIzq_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => MoverSelector(-1);

    private void BtnDer_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        => MoverSelector(+1);

    private void BtnSoltar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _engine.SueltaFicha(_columnaActual);
        RedibujarTablero();
    }

    private void BtnReiniciar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _engine.Reincia();
        _columnaActual = 3;
        RedibujarTablero();
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)  MoverSelector(-1);
        if (e.Key == Key.Right) MoverSelector(+1);
        if (e.Key == Key.Space || e.Key == Key.Down) BtnSoltar_Click(sender, new());
    }
}