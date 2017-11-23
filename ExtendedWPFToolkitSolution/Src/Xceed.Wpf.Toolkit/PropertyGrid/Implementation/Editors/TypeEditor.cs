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
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Attributes;

namespace Xceed.Wpf.Toolkit.PropertyGrid.Editors
{
  public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
  {
    #region Properties

    protected T Editor
    {
      get;
      set;
    }
    protected DependencyProperty ValueProperty
    {
      get;
      set;
    }

    #endregion //Properties

    #region ITypeEditor Members

    public virtual FrameworkElement ResolveEditor( PropertyItem propertyItem )
    {
      Editor = this.CreateEditor();
      SetValueDependencyProperty();
      SetControlProperties( propertyItem );
      ResolveValueBinding( propertyItem );
            #region IUEditor
            ResolveIsEnabledBinding(propertyItem);
            #endregion // IUEditor
      return Editor;
    }

    #endregion //ITypeEditor Members

    #region Methods

    protected virtual T CreateEditor()
    {
      return new T();
    }

    protected virtual IValueConverter CreateValueConverter(PropertyItem propertyItem)
    {
        #region IUEditor
        var valueConterterAttribute = PropertyGridUtilities.GetAttribute<ValueConverterAttribute>(propertyItem.DescriptorDefinition.PropertyDescriptor);
        if (valueConterterAttribute != null)
        {
            return (IValueConverter)Activator.CreateInstance(valueConterterAttribute.ConverterType);
        }

        return null;

        #endregion // IUEditor
    }

    protected virtual void ResolveValueBinding( PropertyItem propertyItem )
    {
      var _binding = new Binding( "Value" );
      _binding.Source = propertyItem;
      _binding.UpdateSourceTrigger = (Editor is InputBase) ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.Default;
      _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
      _binding.Converter = CreateValueConverter(propertyItem);
       BindingOperations.SetBinding( Editor, ValueProperty, _binding );
    }

        #region IUEditor
        protected void ResolveIsEnabledBinding(PropertyItem propertyItem)
        {
            var _binding = new Binding("IsEnabled")
            {
                Source = propertyItem,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                Mode = BindingMode.OneWay
            };
            BindingOperations.SetBinding(Editor, UIElement.IsEnabledProperty, _binding);
        }
        #endregion // IUEditor

    protected virtual void SetControlProperties( PropertyItem propertyItem )
    {
      //TODO: implement in derived class
    }

    protected abstract void SetValueDependencyProperty();

    #endregion //Methods
  }
}
