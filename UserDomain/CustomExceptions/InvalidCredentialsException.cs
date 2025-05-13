using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.CustomExceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Incorect login or password") { }
}
