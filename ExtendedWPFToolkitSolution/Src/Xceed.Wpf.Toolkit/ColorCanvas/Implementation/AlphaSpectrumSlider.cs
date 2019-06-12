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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace Xceed.Wpf.Toolkit
{
  [TemplatePart( Name = PART_SpectrumDisplay, Type = typeof( Rectangle ) )]
  public class AlphaSpectrumSlider : Slider
  {
    private const string PART_SpectrumDisplay = "PART_SpectrumDisplay";

    #region Private Members

    private Rectangle _spectrumDisplay;
    private LinearGradientBrush _pickerBrush;
    private Color? _selectedColor;

    #endregion //Private Members

    #region Constructors

    static AlphaSpectrumSlider()
    {
      DefaultStyleKeyProperty.OverrideMetadata( typeof( AlphaSpectrumSlider ), new FrameworkPropertyMetadata( typeof( AlphaSpectrumSlider ) ) );
    }

    #endregion //Constructors

    #region Dependency Properties


    public Color? SelectedColor
    {
      get
      {
        return _selectedColor;
      }
      set
      {
        Value = value.HasValue ? value.Value.ScA * 100 : 100;
        _selectedColor = value;
        UpdateSpectrum();
      }
    }

    #endregion //Dependency Properties

    #region Base Class Overrides

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _spectrumDisplay = ( Rectangle )GetTemplateChild( PART_SpectrumDisplay );
      UpdateSpectrum();
      OnValueChanged( Double.NaN, Value );
    }


    #endregion //Base Class Overrides

    #region Methods

    private void UpdateSpectrum()
    {
      _pickerBrush = new LinearGradientBrush();

      Color currentcolor = SelectedColor ?? Colors.Black;

      // This gradient stop is partially transparent.
      _pickerBrush.GradientStops.Add(
          new GradientStop( Color.FromArgb( 255, currentcolor.R, currentcolor.G, currentcolor.B ), 0.0 ) );

      // This gradient stop is fully opaque. 
      _pickerBrush.GradientStops.Add(
          new GradientStop( Color.FromArgb( 0, currentcolor.R, currentcolor.G, currentcolor.B ), 1.0 ) );

      if (_spectrumDisplay != null)
      {
        _spectrumDisplay.Fill = _pickerBrush;
      }
    }

    #endregion //Methods
  }
}
