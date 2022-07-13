using System;
using System.Collections.Generic;
using System.Text;

namespace TBS.PrintTest.Business.ErrorHandling
{

    public enum ErrorType
    {
        RecordNotFound = 1,
        BusinessLogic= 2
    }

    public class BusinessResult
    {
        public bool Success { get; set; } = true;
        public ErrorType ErrorType { get; set; }

        public List<string> ErrorMessages { get; set; } = new List<string>();


        internal void AddError(string errorMessage, ErrorType errorType)
        {
            Success = false;
            ErrorType = errorType;
            ErrorMessages.Add(errorMessage);
        }
        
    }
    public class BusinessResult<T> : BusinessResult
    {
        public T Value
        {
            get; set;
        }
    }
}
