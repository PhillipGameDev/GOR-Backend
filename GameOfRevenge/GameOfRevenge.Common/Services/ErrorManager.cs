using System;

namespace GameOfRevenge.Common.Services
{
    public class ErrorManager
    {
        public static string ShowError(Exception ex)
        {
            if (typeof(InvalidModelExecption) == ex.GetType()) return ex.Message;
            if (typeof(DataNotExistExecption) == ex.GetType()) return ex.Message;
            if (typeof(RequirementExecption) == ex.GetType()) return ex.Message;
#if DEBUG
            else return ex.Message;
#else
            else return ShowError();
#endif
        }

        public static string ShowError()
        {
            return "Unexpected Error Occured";
        }
    }
}
