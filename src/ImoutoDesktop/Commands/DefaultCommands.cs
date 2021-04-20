using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ImoutoDesktop.Commands
{
    public class ExitCommand : ICommand
    {
        #region ICommand メンバ

        public Priority Priority
        {
            get { return Priority.Lowest; }
        }

        public string EventID { get; set; }

        public string[] Parameters { get; set; }

        public void Initialize(string path)
        {
        }

        public void Uninitialize()
        {
        }

        public bool IsExecute(string input)
        {
            return input.Contains("終了");
        }

        public bool PreExecute(string input)
        {
            EventID = "Close";
            return true;
        }

        public bool Execute(string input, out string result)
        {
            result = null;
            EventID = "Close";
            return true;
        }

        #endregion
    }
}
