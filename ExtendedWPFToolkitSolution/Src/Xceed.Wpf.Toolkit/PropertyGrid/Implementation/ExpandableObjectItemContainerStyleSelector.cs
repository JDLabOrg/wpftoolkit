/*************************************************************************************

   Added 2019-05-06
   by IUEditor

  ***********************************************************************************/

using System.Windows;
using System.Windows.Controls;

namespace Xceed.Wpf.Toolkit.PropertyGrid
{
  public class ExpandableObjectItemContainerStyleSelector : StyleSelector
  {

    /// <summary>
    /// Set outside (xaml)
    /// </summary>
    public Style TitleContainerStyle
    {
      get;
      set;
    }

    public override Style SelectStyle( object item, DependencyObject container )
    {
      PropertyItem pItem = item as PropertyItem;
      if (pItem.Editor == null && pItem.IsExpandable)
      {
        return TitleContainerStyle;

      }

      return base.SelectStyle( item, container );

    }
  }
}
