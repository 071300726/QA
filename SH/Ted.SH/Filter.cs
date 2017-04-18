using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ted.SH
{
    public abstract class Filter
    {
        public abstract bool IsMatch(string str);
    }

    public class RegexFilter : Filter
    {
        private const string SEPARATOR = @"[\s\t\r\n;]*";
        private Regex _regex;

        public int Code { get; private set; }

        public RegexFilter(string regex, int code)
        {
            Code = code;
            _regex = new Regex(SEPARATOR + regex + SEPARATOR, RegexOptions.Compiled);
        }

        public override bool IsMatch(string str)
        {
            return _regex.IsMatch(str);
        }
    }

    public class CommonFilter :Filter
    {
        public override bool IsMatch(string str)
        {
 	        throw new NotImplementedException();
        }
    }

}
