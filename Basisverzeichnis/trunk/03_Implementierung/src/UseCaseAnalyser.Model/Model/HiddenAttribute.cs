using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphFramework;

namespace UseCaseAnalyser.Model.Model
{
    /// <summary>
    /// a marker class for attributes to filter some attributes from displaying in the view
    /// </summary>
    public class HiddenAttribute : Attribute
    {
        /// <summary>
        /// initializes the attribute with its name and its value
        /// </summary>
        /// <param name="name">the name of the attribute</param>
        /// <param name="value">the value of the attribute</param>
        public HiddenAttribute(string name, object value)
            : base(name, value)
        {
        }
    }
}
