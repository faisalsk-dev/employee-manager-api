using System.Runtime.CompilerServices;

namespace EmployeeManager.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message):base(message) 
        {
        } 
    }
}
