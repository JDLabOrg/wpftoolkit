using System;


/// <summary>
/// Custom atributes by IUEditor 
/// </summary>
namespace Xceed.Wpf.Toolkit.PropertyGrid.Attributes
{
    public class DesignatedObjectAttribute  : Attribute
    {
        private object _value;

        #region Construtors
        public DesignatedObjectAttribute(object value)
        {
            _value = value;
        }

    #endregion

    #region Properties

        public virtual object Value
        {
            get
            {
                return _value;
            }
        }

    #endregion

    }

}
