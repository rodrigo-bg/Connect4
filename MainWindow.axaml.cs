using System;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
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
    private bool _animando;

    private readonly Connect4Engine _engine = new();

    private static readonly IBrush ColorVacio = new SolidColorBrush(Color.Parse("#12122E"));
    private static readonly IBrush ColorSelector = Brushes.DarkBlue;
    private static readonly IBrush BordeVacio = new SolidColorBrush(Color.Parse("#2A2A55"));
    private static readonly IBrush ColorRojo = new SolidColorBrush(Color.Parse("#E74C3C"));
    private static readonly IBrush BordeRojo = new SolidColorBrush(Color.Parse("#96281B"));
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
            TableroBorder.IsVisible = false; // se borra el tablero
            PantallaGanador.IsVisible = true; // aparece GANASTE + REINICIAR
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
    
    // Cuánto se desplaza el grupo palito+perilla desde el centro (en px)
    private const double PalIzquierda = -24;
    private const double PalCentro = 0;
    private const double PalDerecha = 24;

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (_engine.JuegoTerminado) return;

        if ((e.Key == Key.Left && _engine.TurnoActual == Ficha.Amarillo) || 
             (e.Key == Key.A &&  _engine.TurnoActual == Ficha.Rojo ))
        {
            MoverSelector(-1);
            Canvas.SetLeft(Palanquita, PalIzquierda);
        }
        else if ((e.Key == Key.Right && _engine.TurnoActual == Ficha.Amarillo)||
                 (e.Key == Key.D && _engine.TurnoActual == Ficha.Rojo))
        {
            MoverSelector(+1);
            Canvas.SetLeft(Palanquita, PalDerecha);
        }
        else if ((e.Key == Key.Down && _engine.TurnoActual == Ficha.Amarillo) || 
                 (e.Key == Key.S && _engine.TurnoActual == Ficha.Rojo))
        {
            BtnSoltar.BorderThickness =  new Thickness(15);
            BtnSoltar_Click(sender, new());
        }
    }

    private void Window_KeyUp(object? sender, KeyEventArgs e)
    {
        // Al soltar la flecha, la palanquita vuelve al centro (quieta)
        if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.A  || e.Key == Key.S || e.Key == Key.D)
            Canvas.SetLeft(Palanquita, PalCentro);
        if(e.Key == Key.Space || e.Key == Key.Down || e.Key == Key.S)
            BtnSoltar.BorderThickness = new Thickness(10);
    }

    private async void BtnSoltar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_animando) return;

        Ficha fichaQueCae = _engine.TurnoActual;
        int fila = _engine.SueltaFicha(_columnaActual);
        if (fila == -1) return;

        _animando = true;
        await AnimarCaida(_columnaActual, fila, fichaQueCae);
        _animando = false;

        RedibujarTablero();
    }

    // Anima una ficha cayendo desde arriba del tablero hasta la fila donde quedó
    private async Task AnimarCaida(int columna, int filaDestino, Ficha ficha)
    {
        const double celda = 56; // 48 de la ficha + 4 de margen a cada lado
        const double margen = 4;

        var pieza = new Ellipse
        {
            Width = 48, Height = 48,
            Fill = ficha == Ficha.Rojo ? ColorRojo : ColorAmarillo,
            Stroke = ficha == Ficha.Rojo ? BordeRojo : BordeAmarillo,
            StrokeThickness = 2,
            Transitions = new Transitions
            {
                new DoubleTransition
                {
                    Property = Canvas.TopProperty,
                    Duration = TimeSpan.FromMilliseconds(50 + filaDestino * 60),
                    Easing = new CubicEaseIn()
                }
            }
        };

        Canvas.SetLeft(pieza, columna * celda + margen);
        Canvas.SetTop(pieza, -celda);
        CapaAnimacion.Children.Add(pieza);
        await Task.Delay(16); // deja pintar la posición inicial antes de animar
        Canvas.SetTop(pieza, filaDestino * celda + margen);
        await Task.Delay(TimeSpan.FromMilliseconds(50 + filaDestino * 60));

        CapaAnimacion.Children.Remove(pieza);
    }

    private void BtnReiniciar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _engine.Reincia();
        _columnaActual = 3;
        RedibujarTablero();
    }

}