using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUse
{
    public class InvalidCityDataException : Exception
    {
        public InvalidCityDataException(string message)
        : base(message)
        {
        }
    }
}
