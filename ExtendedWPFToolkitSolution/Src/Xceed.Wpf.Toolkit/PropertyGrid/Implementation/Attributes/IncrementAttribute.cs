using System;

namespace Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Attributes
{
    #region IUEditor
    public class IncrementAttribute : Attribute
    {
        public IncrementAttribute(object increment)
        {
            Increment = increment;
        }
        public object Increment { get; private set; }
    }
    #endregion // IUEditor
}