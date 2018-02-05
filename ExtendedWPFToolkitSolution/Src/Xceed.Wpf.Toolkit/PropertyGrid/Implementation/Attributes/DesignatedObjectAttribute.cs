using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Custom atributes by IUEditor 
/// </summary>
namespace Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Attributes
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
