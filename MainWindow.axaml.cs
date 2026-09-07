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

    // Colores en un solo lugar.
    private static readonly IBrush ColorVacio    = new SolidColorBrush(Color.Parse("#12122E"));
    private static readonly IBrush ColorSelector = Brushes.Snow;
    private static readonly IBrush BordeVacio    = new SolidColorBrush(Color.Parse("#2A2A55"));

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

    // Repinta todo: vacío por defecto, blanco en la columna seleccionada.
    private void RedibujarTablero()
    {
        for (int f = 0; f < Filas; f++)
        {
            for (int c = 0; c < Columnas; c++)
            {
                bool seleccionada = (c == _columnaActual);
                _celdas[f, c].Fill = seleccionada ? ColorSelector : ColorVacio;
                _celdas[f, c].Stroke = seleccionada ? ColorSelector : BordeVacio;
            }
        }
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
        // Paso 2: aquí va la lógica de soltar la ficha.
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Left)  MoverSelector(-1);
        if (e.Key == Key.Right) MoverSelector(+1);
        if (e.Key == Key.Space || e.Key == Key.Enter) BtnSoltar_Click(sender, new());
    }
}
