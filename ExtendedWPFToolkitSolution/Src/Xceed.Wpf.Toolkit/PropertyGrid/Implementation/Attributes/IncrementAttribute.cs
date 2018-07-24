using System;

namespace Xceed.Wpf.Toolkit.PropertyGrid.Attributes
{
    #region IUEditor
    /// <summary>
    /// This is attribute for Increment property such as NumericUpDown.Increment
    /// </summary>
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