using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUse
{
    public class KeyNotExistException : Exception
    {
        public KeyNotExistException() { }
        public KeyNotExistException(string message) : base(message)
        {
        }

        // Constructor with custom message and inner exception
        public KeyNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
