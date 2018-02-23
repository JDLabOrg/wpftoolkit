/*************************************************************************************

   Extended WPF Toolkit

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the Microsoft Public
   License (Ms-PL) as published at http://wpftoolkit.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up the Plus Edition at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like http://facebook.com/datagrids

  ***********************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;

namespace Xceed.Wpf.Toolkit
{
  public enum ColorMode
  {
    ColorPalette,
    ColorCanvas
  }

  public enum ColorSortingMode
  {
    Alphabetical,
    HueSaturationBrightness
  }

  [TemplatePart( Name = PART_AvailableColors, Type = typeof( ListBox ) )]
  [TemplatePart( Name = PART_StandardColors, Type = typeof( ListBox ) )]
  [TemplatePart( Name = PART_RecentColors, Type = typeof( ListBox ) )]
  [TemplatePart( Name = PART_ColorPickerToggleButton, Type = typeof( ToggleButton ) )]
  [TemplatePart( Name = PART_ColorPickerPalettePopup, Type = typeof( Popup ) )]
  public class ColorPicker : Control
  {
    private const string PART_AvailableColors = "PART_AvailableColors";
    private const string PART_StandardColors = "PART_StandardColors";
    private const string PART_RecentColors = "PART_RecentColors";
    private const string PART_ColorPickerToggleButton = "PART_ColorPickerToggleButton";
    private const string PART_ColorPickerPalettePopup = "PART_ColorPickerPalettePopup";

    #region Members

    private ListBox _availableColors;
    private ListBox _standardColors;
    private ListBox _recentColors;
    private ToggleButton _toggleButton;
    private Popup _popup;
    private Color? _initialColor;
    private bool _selectionChanged;

    #endregion //Members

    #region Properties

    #region AdvancedButtonHeader

    public static readonly DependencyProperty AdvancedButtonHeaderProperty = DependencyProperty.Register( "AdvancedButtonHeader", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "Web Colors" ) );
    public string AdvancedButtonHeader
    {
      get
      {
        return ( string )GetValue( AdvancedButtonHeaderProperty );
      }
      set
      {
        SetValue( AdvancedButtonHeaderProperty, value );
      }
    }

    #endregion //AdvancedButtonHeader

    #region AvailableColors

    public static readonly DependencyProperty AvailableColorsProperty = DependencyProperty.Register( "AvailableColors", typeof( ObservableCollection<ColorItem> ), typeof( ColorPicker ), new UIPropertyMetadata( CreateAvailableColors() ) );
    public ObservableCollection<ColorItem> AvailableColors
    {
      get
      {
        return ( ObservableCollection<ColorItem> )GetValue( AvailableColorsProperty );
      }
      set
      {
        SetValue( AvailableColorsProperty, value );
      }
    }

    #endregion //AvailableColors

    #region AvailableColorsSortingMode

    public static readonly DependencyProperty AvailableColorsSortingModeProperty = DependencyProperty.Register( "AvailableColorsSortingMode", typeof( ColorSortingMode ), typeof( ColorPicker ), new UIPropertyMetadata( ColorSortingMode.Alphabetical, OnAvailableColorsSortingModeChanged ) );
    public ColorSortingMode AvailableColorsSortingMode
    {
      get
      {
        return ( ColorSortingMode )GetValue( AvailableColorsSortingModeProperty );
      }
      set
      {
        SetValue( AvailableColorsSortingModeProperty, value );
      }
    }

    private static void OnAvailableColorsSortingModeChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ColorPicker colorPicker = ( ColorPicker )d;
      if( colorPicker != null )
        colorPicker.OnAvailableColorsSortingModeChanged( ( ColorSortingMode )e.OldValue, ( ColorSortingMode )e.NewValue );
    }

    private void OnAvailableColorsSortingModeChanged( ColorSortingMode oldValue, ColorSortingMode newValue )
    {
      ListCollectionView lcv = ( ListCollectionView )( CollectionViewSource.GetDefaultView( this.AvailableColors ) );
      if( lcv != null )
      {
        lcv.CustomSort = ( AvailableColorsSortingMode == ColorSortingMode.HueSaturationBrightness )
                          ? new ColorSorter()
                          : null;
      }
    }

    #endregion //AvailableColorsSortingMode

    #region AvailableColorsHeader

    public static readonly DependencyProperty AvailableColorsHeaderProperty = DependencyProperty.Register( "AvailableColorsHeader", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "Colors" ) );
    public string AvailableColorsHeader
    {
      get
      {
        return ( string )GetValue( AvailableColorsHeaderProperty );
      }
      set
      {
        SetValue( AvailableColorsHeaderProperty, value );
      }
    }

    #endregion //AvailableColorsHeader

    #region ButtonStyle

    public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register( "ButtonStyle", typeof( Style ), typeof( ColorPicker ) );
    public Style ButtonStyle
    {
      get
      {
        return ( Style )GetValue( ButtonStyleProperty );
      }
      set
      {
        SetValue( ButtonStyleProperty, value );
      }
    }

    #endregion //ButtonStyle

    #region DisplayColorAndName

    public static readonly DependencyProperty DisplayColorAndNameProperty = DependencyProperty.Register( "DisplayColorAndName", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( false ) );
    public bool DisplayColorAndName
    {
      get
      {
        return ( bool )GetValue( DisplayColorAndNameProperty );
      }
      set
      {
        SetValue( DisplayColorAndNameProperty, value );
      }
    }

    #endregion //DisplayColorAndName

    #region ColorMode

    public static readonly DependencyProperty ColorModeProperty = DependencyProperty.Register( "ColorMode", typeof( ColorMode ), typeof( ColorPicker ), new UIPropertyMetadata( ColorMode.ColorPalette ) );
    public ColorMode ColorMode
    {
      get
      {
        return ( ColorMode )GetValue( ColorModeProperty );
      }
      set
      {
        SetValue( ColorModeProperty, value );
      }
    }

    #endregion //ColorMode

    #region IsOpen

    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register( "IsOpen", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( false, OnIsOpenChanged ) );
    public bool IsOpen
    {
      get
      {
        return ( bool )GetValue( IsOpenProperty );
      }
      set
      {
        SetValue( IsOpenProperty, value );
      }
    }

    private static void OnIsOpenChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ColorPicker colorPicker = (ColorPicker)d;
      if( colorPicker != null )
        colorPicker.OnIsOpenChanged( (bool)e.OldValue, (bool)e.NewValue );
    }

    private void OnIsOpenChanged( bool oldValue, bool newValue )
    {
      if( newValue )
      {
        _initialColor = this.SelectedColor;
      }
      RoutedEventArgs args = new RoutedEventArgs( newValue ? OpenedEvent : ClosedEvent, this );
      this.RaiseEvent( args );
    }

    #endregion //IsOpen

    #region MaxDropDownWidth

    public static readonly DependencyProperty MaxDropDownWidthProperty = DependencyProperty.Register( "MaxDropDownWidth", typeof( double )
      , typeof( ColorPicker ), new UIPropertyMetadata( 214d ) );
    public double MaxDropDownWidth
    {
      get
      {
        return ( double )GetValue( MaxDropDownWidthProperty );
      }
      set
      {
        SetValue( MaxDropDownWidthProperty, value );
      }
    }

    private static void OnMaxDropDownWidthChanged( DependencyObject o, DependencyPropertyChangedEventArgs e )
    {
      var colorPicker = o as ColorPicker;
      if( colorPicker != null )
        colorPicker.OnMaxDropDownWidthChanged( ( double )e.OldValue, ( double )e.NewValue );
    }

    protected virtual void OnMaxDropDownWidthChanged( double oldValue, double newValue )
    {

    }

    #endregion

    #region RecentColors

    public static readonly DependencyProperty RecentColorsProperty = DependencyProperty.Register( "RecentColors", typeof( ObservableCollection<ColorItem> ), typeof( ColorPicker ), new UIPropertyMetadata( null ) );
    public ObservableCollection<ColorItem> RecentColors
    {
      get
      {
        return ( ObservableCollection<ColorItem> )GetValue( RecentColorsProperty );
      }
      set
      {
        SetValue( RecentColorsProperty, value );
      }
    }

    #endregion //RecentColors

    #region RecentColorsHeader

    public static readonly DependencyProperty RecentColorsHeaderProperty = DependencyProperty.Register( "RecentColorsHeader", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "Recent Colors" ) );
    public string RecentColorsHeader
    {
      get
      {
        return ( string )GetValue( RecentColorsHeaderProperty );
      }
      set
      {
        SetValue( RecentColorsHeaderProperty, value );
      }
    }

    #endregion //RecentColorsHeader

    #region SelectedColor

    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register( "SelectedColor", typeof( Color? ), typeof( ColorPicker ), new FrameworkPropertyMetadata( null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback( OnSelectedColorPropertyChanged ) ) );
    public Color? SelectedColor
    {
      get
      {
        return ( Color? )GetValue( SelectedColorProperty );
      }
      set
      {
        SetValue( SelectedColorProperty, value );
      }
    }

    private static void OnSelectedColorPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ColorPicker colorPicker = ( ColorPicker )d;
      if( colorPicker != null )
        colorPicker.OnSelectedColorChanged( ( Color? )e.OldValue, ( Color? )e.NewValue );
    }

    private void OnSelectedColorChanged( Color? oldValue, Color? newValue )
    {
      SelectedColorText = GetFormatedColorString( newValue );

      RoutedPropertyChangedEventArgs<Color?> args = new RoutedPropertyChangedEventArgs<Color?>( oldValue, newValue );
      args.RoutedEvent = ColorPicker.SelectedColorChangedEvent;
      RaiseEvent( args );
    }

    #endregion //SelectedColor

    #region SelectedColorText

    public static readonly DependencyProperty SelectedColorTextProperty = DependencyProperty.Register( "SelectedColorText", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "" ) );
    public string SelectedColorText
    {
      get
      {
        return ( string )GetValue( SelectedColorTextProperty );
      }
      protected set
      {
        SetValue( SelectedColorTextProperty, value );
      }
    }

    #endregion //SelectedColorText

    #region ShowTabHeaders

    public static readonly DependencyProperty ShowTabHeadersProperty = DependencyProperty.Register( "ShowTabHeaders", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( true ) );
    public bool ShowTabHeaders
    {
      get
      {
        return ( bool )GetValue( ShowTabHeadersProperty );
      }
      set
      {
        SetValue( ShowTabHeadersProperty, value );
      }
    }

    #endregion //ShowTabHeaders

    #region ShowAvailableColors

    public static readonly DependencyProperty ShowAvailableColorsProperty = DependencyProperty.Register( "ShowAvailableColors", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( false ) );
    public bool ShowAvailableColors
    {
      get
      {
        return ( bool )GetValue( ShowAvailableColorsProperty );
      }
      set
      {
        SetValue( ShowAvailableColorsProperty, value );
      }
    }

    #endregion //ShowAvailableColors

    #region ShowRecentColors

    public static readonly DependencyProperty ShowRecentColorsProperty = DependencyProperty.Register( "ShowRecentColors", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( true ) );
    public bool ShowRecentColors
    {
      get
      {
        return ( bool )GetValue( ShowRecentColorsProperty );
      }
      set
      {
        SetValue( ShowRecentColorsProperty, value );
      }
    }

    #endregion //DisplayRecentColors

    #region ShowStandardColors

    public static readonly DependencyProperty ShowStandardColorsProperty = DependencyProperty.Register( "ShowStandardColors", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( true ) );
    public bool ShowStandardColors
    {
      get
      {
        return ( bool )GetValue( ShowStandardColorsProperty );
      }
      set
      {
        SetValue( ShowStandardColorsProperty, value );
      }
    }

    #endregion //DisplayStandardColors

    #region ShowDropDownButton

    public static readonly DependencyProperty ShowDropDownButtonProperty = DependencyProperty.Register( "ShowDropDownButton", typeof( bool ), typeof( ColorPicker ), new UIPropertyMetadata( true ) );
    public bool ShowDropDownButton
    {
      get
      {
        return ( bool )GetValue( ShowDropDownButtonProperty );
      }
      set
      {
        SetValue( ShowDropDownButtonProperty, value );
      }
    }

    #endregion //ShowDropDownButton

    #region StandardButtonHeader

    public static readonly DependencyProperty StandardButtonHeaderProperty = DependencyProperty.Register( "StandardButtonHeader", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "Standard" ) );
    public string StandardButtonHeader
    {
      get
      {
        return ( string )GetValue( StandardButtonHeaderProperty );
      }
      set
      {
        SetValue( StandardButtonHeaderProperty, value );
      }
    }

    #endregion //StandardButtonHeader

    #region StandardColors

    public static readonly DependencyProperty StandardColorsProperty = DependencyProperty.Register( "StandardColors", typeof( ObservableCollection<ColorItem> ), typeof( ColorPicker ), new UIPropertyMetadata( CreateStandardColors() ) );
    public ObservableCollection<ColorItem> StandardColors
    {
      get
      {
        return ( ObservableCollection<ColorItem> )GetValue( StandardColorsProperty );
      }
      set
      {
        SetValue( StandardColorsProperty, value );
      }
    }

    #endregion //StandardColors

    #region StandardColorsHeader

    public static readonly DependencyProperty StandardColorsHeaderProperty = DependencyProperty.Register( "StandardColorsHeader", typeof( string ), typeof( ColorPicker ), new UIPropertyMetadata( "Standard Colors" ) );
    public string StandardColorsHeader
    {
      get
      {
        return ( string )GetValue( StandardColorsHeaderProperty );
      }
      set
      {
        SetValue( StandardColorsHeaderProperty, value );
      }
    }

    #endregion //StandardColorsHeader

    #region UsingAlphaChannel

    public static readonly DependencyProperty UsingAlphaChannelProperty = DependencyProperty.Register( "UsingAlphaChannel", typeof( bool ), typeof( ColorPicker ), new FrameworkPropertyMetadata( true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback( OnUsingAlphaChannelPropertyChanged ) ) );
    public bool UsingAlphaChannel
    {
      get
      {
        return ( bool )GetValue( UsingAlphaChannelProperty );
      }
      set
      {
        SetValue( UsingAlphaChannelProperty, value );
      }
    }

    private static void OnUsingAlphaChannelPropertyChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
    {
      ColorPicker colorPicker = ( ColorPicker )d;
      if( colorPicker != null )
        colorPicker.OnUsingAlphaChannelChanged();
    }

    private void OnUsingAlphaChannelChanged()
    {
      SelectedColorText = GetFormatedColorString( SelectedColor );
    }

    #endregion //UsingAlphaChannel

    #endregion //Properties

    #region Constructors

    static ColorPicker()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( ColorPicker ), new FrameworkPropertyMetadata( typeof( ColorPicker ) ) );
    }

    public ColorPicker()
    {
#if VS2008
        this.RecentColors = new ObservableCollection<ColorItem>();
#else
      this.SetCurrentValue( ColorPicker.RecentColorsProperty, new ObservableCollection<ColorItem>() );
#endif

      Keyboard.AddKeyDownHandler( this, OnKeyDown );
      Mouse.AddPreviewMouseDownOutsideCapturedElementHandler( this, OnMouseDownOutsideCapturedElement );
    }

    #endregion //Constructors

    #region Base Class Overrides

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if( _availableColors != null )
        _availableColors.SelectionChanged -= Color_SelectionChanged;

      _availableColors = GetTemplateChild( PART_AvailableColors ) as ListBox;
      if( _availableColors != null )
        _availableColors.SelectionChanged += Color_SelectionChanged;

      if( _standardColors != null )
        _standardColors.SelectionChanged -= Color_SelectionChanged;

      _standardColors = GetTemplateChild( PART_StandardColors ) as ListBox;
      if( _standardColors != null )
        _standardColors.SelectionChanged += Color_SelectionChanged;

      if( _recentColors != null )
        _recentColors.SelectionChanged -= Color_SelectionChanged;

      _recentColors = GetTemplateChild( PART_RecentColors ) as ListBox;
      if( _recentColors != null )
        _recentColors.SelectionChanged += Color_SelectionChanged;

      if( _popup != null )
        _popup.Opened -= Popup_Opened;

      _popup = GetTemplateChild( PART_ColorPickerPalettePopup ) as Popup;
      if( _popup != null )
        _popup.Opened += Popup_Opened;

      _toggleButton = this.Template.FindName( PART_ColorPickerToggleButton, this ) as ToggleButton;
    }

    protected override void OnMouseUp( MouseButtonEventArgs e )
    {
      base.OnMouseUp( e );

      // Close ColorPicker on MouseUp to prevent action of mouseUp on controls behind the ColorPicker.
      if( _selectionChanged )
      {
        CloseColorPicker( true );
        _selectionChanged = false;
      }
    }

    #endregion //Base Class Overrides

    #region Event Handlers

    private void OnKeyDown( object sender, KeyEventArgs e )
    {
      if( !IsOpen )
      {
        if( KeyboardUtilities.IsKeyModifyingPopupState( e ) )
        {
          IsOpen = true;
          // Focus will be on ListBoxItem in Popup_Opened().
          e.Handled = true;
        }
      }
      else
      {
        if( KeyboardUtilities.IsKeyModifyingPopupState( e ) )
        {
          CloseColorPicker( true );
          e.Handled = true;
        }
        else if( e.Key == Key.Escape )
        {
          this.SelectedColor = _initialColor;
          CloseColorPicker( true );
          e.Handled = true;
        }
      }
    }

    private void OnMouseDownOutsideCapturedElement( object sender, MouseButtonEventArgs e )
    {
      CloseColorPicker( true );
    }

    private void Color_SelectionChanged( object sender, SelectionChangedEventArgs e )
    {
      ListBox lb = ( ListBox )sender;

      if( e.AddedItems.Count > 0 )
      {
        var colorItem = ( ColorItem )e.AddedItems[ 0 ];
        SelectedColor = colorItem.Color;
        if( !string.IsNullOrEmpty( colorItem.Name ) )
        {
          this.SelectedColorText = colorItem.Name;
        }
        UpdateRecentColors( colorItem );
        _selectionChanged = true;
        lb.SelectedIndex = -1; //for now I don't care about keeping track of the selected color
      }
    }

    private void Popup_Opened( object sender, EventArgs e )
    {
      if( ( _availableColors != null ) && ShowAvailableColors )
      {
        FocusOnListBoxItem( _availableColors );
      }
      else if( ( _standardColors != null ) && ShowStandardColors )
        FocusOnListBoxItem( _standardColors );
      else if( ( _recentColors != null ) && ShowRecentColors )
        FocusOnListBoxItem( _recentColors );
    }

    private void FocusOnListBoxItem( ListBox listBox )
    {
      ListBoxItem listBoxItem = ( ListBoxItem )listBox.ItemContainerGenerator.ContainerFromItem( listBox.SelectedItem );
      if( ( listBoxItem == null ) && ( listBox.Items.Count > 0 ) )
        listBoxItem = ( ListBoxItem )listBox.ItemContainerGenerator.ContainerFromItem( listBox.Items[ 0 ] );
      if( listBoxItem != null )
        listBoxItem.Focus();
    }

    #endregion //Event Handlers

    #region Events

    #region SelectedColorChangedEvent

    public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent( "SelectedColorChanged", RoutingStrategy.Bubble, typeof( RoutedPropertyChangedEventHandler<Color?> ), typeof( ColorPicker ) );
    public event RoutedPropertyChangedEventHandler<Color?> SelectedColorChanged
    {
      add
      {
        AddHandler( SelectedColorChangedEvent, value );
      }
      remove
      {
        RemoveHandler( SelectedColorChangedEvent, value );
      }
    }

    #endregion

    #region OpenedEvent

    public static readonly RoutedEvent OpenedEvent = EventManager.RegisterRoutedEvent( "OpenedEvent", RoutingStrategy.Bubble, typeof( RoutedEventHandler ), typeof( ColorPicker ) );
    public event RoutedEventHandler Opened
    {
      add
      {
        AddHandler( OpenedEvent, value );
      }
      remove
      {
        RemoveHandler( OpenedEvent, value );
      }
    }

    #endregion //OpenedEvent

    #region ClosedEvent

    public static readonly RoutedEvent ClosedEvent = EventManager.RegisterRoutedEvent( "ClosedEvent", RoutingStrategy.Bubble, typeof( RoutedEventHandler ), typeof( ColorPicker ) );
    public event RoutedEventHandler Closed
    {
      add
      {
        AddHandler( ClosedEvent, value );
      }
      remove
      {
        RemoveHandler( ClosedEvent, value );
      }
    }

    #endregion //ClosedEvent

    #endregion //Events

    #region Methods

    private void CloseColorPicker( bool isFocusOnColorPicker )
    {
      if( IsOpen )
        IsOpen = false;
      ReleaseMouseCapture();

      if( isFocusOnColorPicker && ( _toggleButton != null) )
        _toggleButton.Focus();
      this.UpdateRecentColors( new ColorItem( SelectedColor, SelectedColorText ) );
    }

    private void UpdateRecentColors( ColorItem colorItem )
    {
      if( !RecentColors.Contains( colorItem ) )
        RecentColors.Add( colorItem );

      if( RecentColors.Count > 20 ) //don't allow more than 20 (10->20 IUEditor) , maybe make a property that can be set by the user.
        RecentColors.RemoveAt( 0 );
    }

    private string GetFormatedColorString( Color? colorToFormat )
    {
      if( ( colorToFormat == null ) || !colorToFormat.HasValue )
        return string.Empty;

      return ColorUtilities.FormatColorString( colorToFormat.Value.GetColorName(), UsingAlphaChannel );
    }

    private static ObservableCollection<ColorItem> CreateStandardColors()
    {
            // use standard colors as web colors
            ObservableCollection<ColorItem> standardColors = new ObservableCollection<ColorItem>
            {
                // IUEditor 2017-10-23, added null color
                // new ColorItem( null, "No color" ),
                // new ColorItem(Colors.Transparent, "Transparent"),

                // black 1
                new ColorItem(Color.FromRgb(248, 249, 250), "Black1"),
                new ColorItem(Color.FromRgb(241, 243, 245), "Black2"),
                new ColorItem(Color.FromRgb(233, 236, 239), "Black3"),
                new ColorItem(Color.FromRgb(226, 226, 230), "Black4"),
                new ColorItem(Color.FromRgb(206, 212, 218), "Black5"),

                new ColorItem(Color.FromRgb(173, 181, 189), "Black6"),
                new ColorItem(Color.FromRgb(134, 142, 150), "Black7"),
                new ColorItem(Color.FromRgb(73, 80, 87), "Black8"),
                new ColorItem(Color.FromRgb(52, 58, 64), "Black9"),
                new ColorItem(Color.FromRgb(33, 37, 41), "Black10"),

                // red 
                new ColorItem(Color.FromRgb(255, 245, 245), "Red1"),
                new ColorItem(Color.FromRgb(255, 227, 227), "Red2"),
                new ColorItem(Color.FromRgb(255, 201, 201), "Red3"),
                new ColorItem(Color.FromRgb(255, 168, 168), "Red4"),
                new ColorItem(Color.FromRgb(255, 135, 135), "Red5"),

                new ColorItem(Color.FromRgb(255, 107, 107), "Red6"),
                new ColorItem(Color.FromRgb(250, 82, 82), "Red7"),
                new ColorItem(Color.FromRgb(240, 62, 62), "Red8"),
                new ColorItem(Color.FromRgb(224, 49, 49), "Red9"),
                new ColorItem(Color.FromRgb(201, 42, 42), "Red10"),

                // pink 
                new ColorItem(Color.FromRgb(255, 240, 246), "Pink1"),
                new ColorItem(Color.FromRgb(255, 222, 235), "Pink2"),
                new ColorItem(Color.FromRgb(252, 194, 215), "Pink3"),
                new ColorItem(Color.FromRgb(250, 162, 193), "Pink4"),
                new ColorItem(Color.FromRgb(242, 131, 172), "Pink5"),

                new ColorItem(Color.FromRgb(240, 101, 139), "Pink6"),
                new ColorItem(Color.FromRgb(230, 73, 128), "Pink7"),
                new ColorItem(Color.FromRgb(194, 37, 92), "Pink8"),
                new ColorItem(Color.FromRgb(166, 30, 77), "Pink9"),
                new ColorItem(Color.FromRgb(160, 30, 77), "Pink10"),

                // purple
                new ColorItem(Color.FromRgb(248, 240, 252), "Purple1"),
                new ColorItem(Color.FromRgb(243, 217, 250), "Purple2"),
                new ColorItem(Color.FromRgb(238, 190, 250), "Purple3"),
                new ColorItem(Color.FromRgb(229, 153, 247), "Purple4"),
                new ColorItem(Color.FromRgb(218, 119, 242), "Purple5"),

                new ColorItem(Color.FromRgb(204, 93, 232), "Purple6"),
                new ColorItem(Color.FromRgb(190, 75, 219), "Purple7"),
                new ColorItem(Color.FromRgb(174, 62, 201), "Purple8"),
                new ColorItem(Color.FromRgb(156, 54, 181), "Purple9"),
                new ColorItem(Color.FromRgb(134, 46, 156), "Purple10"),

                // BlueyPurple
                new ColorItem(Color.FromRgb(243, 240, 255), "BlueyPurple1"),
                new ColorItem(Color.FromRgb(229, 219, 255), "BlueyPurple2"),
                new ColorItem(Color.FromRgb(208, 191, 255), "BlueyPurple3"),
                new ColorItem(Color.FromRgb(177, 151, 252), "BlueyPurple4"),
                new ColorItem(Color.FromRgb(151, 117, 250), "BlueyPurple5"),

                new ColorItem(Color.FromRgb(132, 94, 247), "BlueyPurple6"),
                new ColorItem(Color.FromRgb(121, 80, 242), "BlueyPurple7"),
                new ColorItem(Color.FromRgb(112, 72, 232), "BlueyPurple8"),
                new ColorItem(Color.FromRgb(103, 65, 217), "BlueyPurple9"),
                new ColorItem(Color.FromRgb(95, 61, 196), "BlueyPurple10"),

                
                // WarmBlue
                new ColorItem(Color.FromRgb(237, 242, 255), "WarmBlue1"),
                new ColorItem(Color.FromRgb(219, 228, 255), "WarmBlue2"),
                new ColorItem(Color.FromRgb(186, 200, 255), "WarmBlue3"),
                new ColorItem(Color.FromRgb(145, 167, 255), "WarmBlue4"),
                new ColorItem(Color.FromRgb(116, 143, 252), "WarmBlue5"),

                new ColorItem(Color.FromRgb(92, 124, 250), "WarmBlue6"),
                new ColorItem(Color.FromRgb(76, 110, 245), "WarmBlue7"),
                new ColorItem(Color.FromRgb(66, 99, 235), "WarmBlue8"),
                new ColorItem(Color.FromRgb(59, 91, 219), "WarmBlue9"),
                new ColorItem(Color.FromRgb(54, 79, 199), "WarmBlue10"),

                // WaterBlue
                new ColorItem(Color.FromRgb(232, 247, 255), "WaterBlue1"),
                new ColorItem(Color.FromRgb(204, 237, 255), "WaterBlue2"),
                new ColorItem(Color.FromRgb(163, 218, 255), "WaterBlue3"),
                new ColorItem(Color.FromRgb(114, 195, 252), "WaterBlue4"),
                new ColorItem(Color.FromRgb(77, 173, 247), "WaterBlue5"),

                new ColorItem(Color.FromRgb(50, 154, 240), "WaterBlue6"),
                new ColorItem(Color.FromRgb(34, 138, 230), "WaterBlue7"),
                new ColorItem(Color.FromRgb(28, 124, 214), "WaterBlue8"),
                new ColorItem(Color.FromRgb(27, 110, 194), "WaterBlue9"),
                new ColorItem(Color.FromRgb(24, 98, 171), "WaterBlue10"),

                 // DeepAqua
                new ColorItem(Color.FromRgb(227, 250, 252), "DeepAqua1"),
                new ColorItem(Color.FromRgb(197, 246, 250), "DeepAqua2"),
                new ColorItem(Color.FromRgb(153, 233, 242), "DeepAqua3"),
                new ColorItem(Color.FromRgb(102, 217, 232), "DeepAqua4"),
                new ColorItem(Color.FromRgb(59, 201, 219), "DeepAqua5"),

                new ColorItem(Color.FromRgb(34, 184, 207), "DeepAqua6"),
                new ColorItem(Color.FromRgb(21, 170, 191), "DeepAqua7"),
                new ColorItem(Color.FromRgb(16, 152, 173), "DeepAqua8"),
                new ColorItem(Color.FromRgb(12, 133, 153), "DeepAqua9"),
                new ColorItem(Color.FromRgb(11, 114, 133), "DeepAqua10"),

                // JungleGreen
                new ColorItem(Color.FromRgb(230, 252, 245), "JungleGreen1"),
                new ColorItem(Color.FromRgb(195, 250, 232), "JungleGreen2"),
                new ColorItem(Color.FromRgb(150, 242, 215), "JungleGreen3"),
                new ColorItem(Color.FromRgb(66, 230, 190), "JungleGreen4"),
                new ColorItem(Color.FromRgb(56, 217, 169), "JungleGreen5"),

                new ColorItem(Color.FromRgb(32, 201, 151), "JungleGreen6"),
                new ColorItem(Color.FromRgb(18, 184, 134), "JungleGreen7"),
                new ColorItem(Color.FromRgb(12, 166, 120), "JungleGreen8"),
                new ColorItem(Color.FromRgb(9, 146, 104), "JungleGreen9"),
                new ColorItem(Color.FromRgb(8, 127, 91), "JungleGreen10"),

                 // Green
                new ColorItem(Color.FromRgb(235, 251, 238), "Green1"),
                new ColorItem(Color.FromRgb(211, 249, 216), "Green2"),
                new ColorItem(Color.FromRgb(178, 242, 187), "Green3"),
                new ColorItem(Color.FromRgb(140, 233, 154), "Green4"),
                new ColorItem(Color.FromRgb(105, 219, 124), "Green5"),

                new ColorItem(Color.FromRgb(81, 207, 102), "Green6"),
                new ColorItem(Color.FromRgb(64, 192, 87), "Green7"),
                new ColorItem(Color.FromRgb(55, 178, 77), "Green8"),
                new ColorItem(Color.FromRgb(47, 158, 68), "Green9"),
                new ColorItem(Color.FromRgb(43, 138, 62), "Green10"),

                  // LawnGreen
                new ColorItem(Color.FromRgb(244, 252, 227), "LawnGreen1"),
                new ColorItem(Color.FromRgb(233, 250, 200), "LawnGreen2"),
                new ColorItem(Color.FromRgb(216, 245, 162), "LawnGreen3"),
                new ColorItem(Color.FromRgb(192, 235, 117), "LawnGreen4"),
                new ColorItem(Color.FromRgb(169, 227, 75), "LawnGreen5"),

                new ColorItem(Color.FromRgb(148, 215, 45), "LawnGreen6"),
                new ColorItem(Color.FromRgb(130, 201, 30), "LawnGreen7"),
                new ColorItem(Color.FromRgb(116, 184, 22), "LawnGreen8"),
                new ColorItem(Color.FromRgb(102, 168, 15), "LawnGreen9"),
                new ColorItem(Color.FromRgb(92, 148, 13), "LawnGreen10"),

                  // Orange 
                new ColorItem(Color.FromRgb(255, 249, 219), "Orange1"),
                new ColorItem(Color.FromRgb(255, 243, 191), "Orange2"),
                new ColorItem(Color.FromRgb(255, 236, 153), "Orange3"),
                new ColorItem(Color.FromRgb(255, 224, 102), "Orange4"),
                new ColorItem(Color.FromRgb(255, 212, 59), "Orange5"),

                new ColorItem(Color.FromRgb(252, 196, 25), "Orange6"),
                new ColorItem(Color.FromRgb(250, 176, 5), "Orange7"),
                new ColorItem(Color.FromRgb(245, 159, 0), "Orange8"),
                new ColorItem(Color.FromRgb(240, 140, 0), "Orange9"),
                new ColorItem(Color.FromRgb(230, 119, 0), "Orange10")
            };
            return standardColors;
    }

    private static ObservableCollection<ColorItem> CreateAvailableColors()
    {
      ObservableCollection<ColorItem> standardColors = new ObservableCollection<ColorItem>();

      foreach( var item in ColorUtilities.KnownColors )
      {
        if( !String.Equals( item.Key, "Transparent" ) )
        {
          var colorItem = new ColorItem( item.Value, item.Key );
          if( !standardColors.Contains( colorItem ) )
            standardColors.Add( colorItem );
        }
      }

      return standardColors;
    }

    #endregion //Methods
  }
}
