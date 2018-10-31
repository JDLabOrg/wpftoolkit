using Gma.System.MouseKeyHook;
using System.Drawing;


namespace Xceed.Wpf.Toolkit.ColorCanvas.Implementation
{

  public delegate void ColorClickedHandler( object sender, System.Windows.Media.Color color );

  class ColorSpoid
  {
    public event ColorClickedHandler ColorClicked;

    static Color GetPixel( Point position )
    {
      using (var bitmap = new Bitmap( 1, 1 ))
      {
        using (var graphics = Graphics.FromImage( bitmap ))
        {
          graphics.CopyFromScreen( position, new Point( 0, 0 ), new Size( 1, 1 ) );
        }
        return bitmap.GetPixel( 0, 0 );
      }
    }


    private IKeyboardMouseEvents m_GlobalHook;

    public void Subscribe()
    {
      // Note: for the application hook, use the Hook.AppEvents() instead
      m_GlobalHook = Hook.GlobalEvents();

      m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
    }

    private void GlobalHookMouseDownExt( object sender, MouseEventExtArgs e )
    {
      // Console.WriteLine( "MouseDown: \t{0}; \t System Timestamp: \t{1}", e.Button, e.Timestamp );

      var color = GetPixel( e.Location );
      // drawing color to media color
      System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(
          color.A, color.R, color.G, color.B );

      ColorClicked?.Invoke( this, selectedColor );

    }

    public void Unsubscribe()
    {
      m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;

      //It is recommened to dispose it
      m_GlobalHook.Dispose();
    }

  }



}

