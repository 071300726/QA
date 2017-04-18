using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Ted.SH
{
    public abstract class TokenFilterBase
    {
        public TokenFilterBase(int code, string text)
        {
            Code = code;
            Text = text;
        }

        public int Code { get; set; }

        public string Text { get; set; }

        public abstract bool IsMatchItem(string str);
        public abstract bool IsPotentialMatchItem(string str);
    }

    public class RegexTokenFilter : TokenFilterBase
    {
        private Regex _regex;

        public int Code { get; private set; }

        public RegexTokenFilter(string regex, int code, string text) 
            : base(code, text)
        {
            _regex = new Regex(regex , RegexOptions.Compiled);
        }

        public override bool IsMatchItem(string str)
        {
            return _regex.IsMatch(str);
        }
        public override bool IsPotentialMatchItem(string str)
        {
            return _regex.IsMatch(str);
        }
    }

    public class StaticTokenFilter : TokenFilterBase
    {
        private string _str;

        public StaticTokenFilter(string str, int code, string text)
            : base(code, text)
        {
            _str = str;
        }

        public override bool IsMatchItem(string str)
        {
            return str == _str;
        }
        public override bool IsPotentialMatchItem(string str)
        {
            return _str.StartsWith(str);
        }
    }

}
