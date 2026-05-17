using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonUse
{
    public class KeyAlreadyExistsException : Exception
    {
        public KeyAlreadyExistsException() : base() { }
        public KeyAlreadyExistsException(string message) : base(message) { }
        public KeyAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }

       
    }
}
