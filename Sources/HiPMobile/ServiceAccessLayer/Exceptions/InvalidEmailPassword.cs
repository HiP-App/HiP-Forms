using System;
using System.Collections.Generic;
using System.Text;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions
{
    class InvalidEmailPassword: Exception 
    {
        public InvalidEmailPassword()
        {
        }

        public InvalidEmailPassword(string message)
            : base(message)
        {
        }

        public InvalidEmailPassword(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
