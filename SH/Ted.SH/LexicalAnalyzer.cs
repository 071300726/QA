using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ted.SH
{
    public class LexicalAnalyzer
    {
        private List<Filter> _filters;

        public void Init(string[] tokenConfigs)
        {
            InitFilters(tokenConfigs);
        }

        private void InitFilters(string[] tokenConfigs)
        {
            _filters = new List<Filter>();
            foreach (var tc in tokenConfigs)
            {
                if (string.IsNullOrEmpty(tc) || tc.StartsWith("#"))
                {
                    continue;
                }
                var items = tc.Split(new char[] { '\t' });
                if (items.Length < 2)
                {
                    continue;
                }
                _filters.Add(new Filter(items[0], int.Parse(items[1])));
            }
        }



        public void Parse(string input)
        {
            int index = -1;
            int length = input.Length;
            
            string current = string.Empty;
            List<Filter> currentFilters = _filters;

            List<Token> tokenList = new List<Token>();

            while (++index < length)
            {
                current += input[index];
                Token token;
                if (Determine(current,ref currentFilters, out token))
                {
                    tokenList.Add(token);
                    current = string.Empty;
                    currentFilters = _filters;
                    index--;
                }
            }

            PrintTokens(tokenList);
        }


        private bool Determine(string current, ref List<Filter> filters, out Token token)
        {
            var newFilters = new List<Filter>();
            foreach (var f in filters)
            {
                if (f.Match(current))
                {
                    newFilters.Add(f);
                }                
            }

            if(newFilters.Count > 0)
            {
                filters = newFilters;
                token = new Token();
                return false;
            }
            else if (filters.Count == 1 && newFilters.Count == 0)
            {
                token = new Token
                {
                    Code = filters[0].Code,
                    Extension = current.Substring(0, current.Length - 1)
                };
                return true;
            }
            else
            {
                throw new Exception("wrong lexical "+ current);
            }
        }

        private void PrintTokens(List<Token> tokenList)
        {
            foreach (var t in tokenList)
            {
                Console.WriteLine(string.Format("[ {0} - {1} ]", t.Code, t.Extension));
            }
        }
    }
}
