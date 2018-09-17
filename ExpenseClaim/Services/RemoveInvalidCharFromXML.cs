using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExpenseClaim.Services.Contract;

namespace ExpenseClaim.Services
{
    public class RemoveInvalidCharFromXML : IRemoveInvalidCharFromXML
    {

        public string GetCleanXML(string requestXML)
        {
            StringBuilder xml = new StringBuilder("<CLAIMS>" + requestXML + "</CLAIMS>");
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
             RegexOptions.IgnoreCase);
            //find items that matches with our pattern
            MatchCollection emailMatches = emailRegex.Matches(xml.ToString());

            for (int i = 0; i <= emailMatches.Count(); i++)
            {
                xml.Replace(emailMatches[0].ToString(), "empty/");
            }

            return xml.ToString();
        }
    }
}
