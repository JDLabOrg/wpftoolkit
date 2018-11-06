using System.Drawing;

namespace Xceed.Wpf.Toolkit
{
  class ColorSpoid
  {
    public static System.Windows.Media.Color GetPixel( System.Windows.Point point )
    {
      Point position = new Point( ( int )point.X, ( int )point.Y );
      using (var bitmap = new Bitmap( 1, 1 ))
      {
        using (var graphics = Graphics.FromImage( bitmap ))
        {
          graphics.CopyFromScreen( position, new Point( 0, 0 ), new Size( 1, 1 ) );
        }

        var color = bitmap.GetPixel( 0, 0 );

        // drawing color to media color
        System.Windows.Media.Color convertedColor = System.Windows.Media.Color.FromArgb(
            color.A, color.R, color.G, color.B );
        return convertedColor;
      }
    }

  }
}
