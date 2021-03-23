using System;

namespace Slime.Helper.Exceptions
{
    public class ConstructorException : Exception 
    {
        public ConstructorException(string s) : base(s)
        {
            
        }
    }
}