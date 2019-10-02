using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLibrary.Interfaces
{
    public interface IAccountValidation
    {
        bool VerifyEmail(string email);
    }
}