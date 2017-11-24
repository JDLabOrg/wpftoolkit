using System;

namespace Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Attributes
{
#region IUEditor
    public class ValueConverterAttribute : Attribute
    {
        public ValueConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
        public Type ConverterType { get; private set; }
    }
#endregion // IUEditor
}
