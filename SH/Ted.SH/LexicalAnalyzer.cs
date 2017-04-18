using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ted.SH
{
    public class LexicalAnalyzer
    {
        private List<TokenFilterBase> _filters;

        public void Init(string[] tokenConfigs)
        {
            InitFilters(tokenConfigs);
        }

        private void InitFilters(string[] tokenConfigs)
        {
            _filters = new List<TokenFilterBase>();
            foreach (var tc in tokenConfigs)
            {
                if (string.IsNullOrEmpty(tc) || tc.StartsWith("#"))
                {
                    continue;
                }
                var items = tc.Split(new char[] { '\t' });
                if (items.Length < 4)
                {
                    continue;
                }
                if (items[1].Equals("regex", StringComparison.InvariantCultureIgnoreCase))
                {
                    _filters.Add(new RegexTokenFilter(items[2], int.Parse(items[3]), items[0]));
                }
                else if (items[1].Equals("static", StringComparison.InvariantCultureIgnoreCase))
                {
                    _filters.Add(new StaticTokenFilter(items[2], int.Parse(items[3]), items[0]));
                }
            }
        }



        public void Parse(string input)
        {
            input += ' ';//增加EOF，否则无法处理最后一个token

            int index = -1;
            int length = input.Length;

            bool catchMode = false;

            bool indexMM = false;

            string current = string.Empty;
            List<TokenFilterBase> matchFilters = _filters;
            List<TokenFilterBase> potentialMatchFilters = _filters;

            List<Token> tokenList = new List<Token>();

            while (++index < length)
            {
                var c = input[index];
                if (catchMode == false && IsBlank(c))
                {
                    continue;
                }
                catchMode = true;
                current += c;
                
                Token token;
                if (Determine(current, ref matchFilters, ref potentialMatchFilters, out token, out indexMM))
                {
                    if (indexMM)
                    {
                        index--;
                    }

                    tokenList.Add(token);
                    current = string.Empty;
                    matchFilters = _filters;
                    potentialMatchFilters = _filters;
                    catchMode = false;
                }
            }

            PrintTokens(tokenList);
        }


        private bool Determine(string current, ref List<TokenFilterBase> lastMatchFilters, ref List<TokenFilterBase> lastPotentialMatchFilters, out Token token, out bool indexMM)
        {
            var matchFilters = new List<TokenFilterBase>();
            var potentialMatchFilters = new List<TokenFilterBase>();
            foreach (var f in lastPotentialMatchFilters)
            {
                if (f.IsMatchItem(current))
                {
                    matchFilters.Add(f);
                }
            }
            foreach (var f in lastPotentialMatchFilters)
            {
                if (f.IsPotentialMatchItem(current))
                {
                    potentialMatchFilters.Add(f);
                }
            }

            if (potentialMatchFilters.Count > 0)
            {
                lastMatchFilters = matchFilters;
                lastPotentialMatchFilters = potentialMatchFilters;
                token = new Token();
                indexMM = false;
                return false;
            }
            else if (matchFilters.Count == 0)
            {
                if (lastMatchFilters.Count == 1)
                {
                    token = new Token
                    {
                        Code = lastMatchFilters[0].Code,
                        Text = lastMatchFilters[0].Text,
                        Extension = current.Substring(0, current.Length - 1)
                    };
                    indexMM = true;
                    return true;
                }
                else
                {
                    throw new Exception("wrong lexical " + current);
                }
            }
            else if (matchFilters.Count == 1)
            {
                token = new Token
                {
                    Code = matchFilters[0].Code,
                    Text = matchFilters[0].Text,
                    Extension = current
                };
                indexMM = false;
                return true;
            }
            else
            {
                throw new Exception("wrong lexical " + current);
            }
        }


        private char[] _blankChars = new char[] { ' ', '\r', '\n', '\t' };
        private bool IsBlank(char c)
        {
            return _blankChars.Contains(c);
        }


        private void PrintTokens(List<Token> tokenList)
        {
            foreach (var t in tokenList)
            {
                Console.WriteLine(string.Format("[{0}] - {1}", t.Extension, t.Code));
            }
        }
    }
}
