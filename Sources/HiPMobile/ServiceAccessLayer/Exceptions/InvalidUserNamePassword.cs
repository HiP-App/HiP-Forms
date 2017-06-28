using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions
    {
    public class InvalidUserNamePassword:Exception
        {
        public InvalidUserNamePassword (){
            }

        public InvalidUserNamePassword (string message)
        : base(message){
            }
        public InvalidUserNamePassword (string message, Exception inner)
        : base(message, inner){
            }
        }
    }
