using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpvin
{
    internal class MessageAnalysis
    {
        public MessageAnalysis() { }


        public (bool isCommand, bool isPolite, bool hasNumber) ScanMessage(String msg)
        {
            bool isCommand = false;
            bool hasNumber = false;

            for (int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == 'v' && i > 1)
                {
                    if (msg[i - 1] == 'e' && msg[i - 2] == 'k')
                    {
                        isCommand = true;
                        continue;
                    }
                }

                if (Char.IsNumber(msg[i]) && hasNumber == false)
                {
                    hasNumber = true;
                    continue;
                }
            }

            bool isPolite = msg.Contains("pleas") || msg.Contains("ples") || msg.Contains("pls");

            return (isCommand, isPolite, hasNumber);
        }

        public (double value, String code_from, String code_to) FindConversion(String msg)
        {
            String[] words = msg.Split(' ');
            String code_from = "", code_to = "";
            double value = 0, tryvalue = 0;
            bool isNumeric, valFound = false, toFound = false;

            for (int i = 0; i < words.Length; i++)
            {
                String w = words[i].Replace('.', ',');
                isNumeric = double.TryParse(w, out tryvalue);

                if (isNumeric && i < words.Length - 1)
                {
                    if (words[i + 1].Length == 3)
                    {
                        value = tryvalue;
                        valFound = true;
                        code_from = words[i + 1];
                    }
                    else
                    {
                        throw new Exception("I didn't understand the request, sorry");
                    }
                }

                if (words[i] == "to" && i < words.Length - 1)
                {
                    if (words[i + 1].Length == 3)
                    {
                        toFound = true;
                        code_to = words[i + 1];
                    }
                    else
                    {
                        throw new Exception("I didn't understand the request, sorry");
                    }
                }
            }

            if (toFound && valFound)
            {
                return (value, code_from, code_to);
            }
            else
            {
                throw new Exception("I didn't understand the request, sorry");
            }
        }


        public List<String> FindOptions(String msg)
        {
            int kev_index = msg.IndexOf("kev");
            String substring = msg.Substring(kev_index);

            String[] words = substring.Split(' ');
            String option = "";
            List<String> options = new List<String>();

            for (int i = 1; i < words.Length; i++)
            {
                words[i] = words[i].Trim('?');
                if (words[i] == "or")
                {
                    if (option != "")
                        options.Add(option);
                    option = "";
                }
                else if (words[i].EndsWith(','))
                {
                    option = option + words[i].Trim(',') + " ";
                    options.Add(option);
                    option = "";
                }
                else
                {
                    option = option + words[i] + " ";
                }
            }
            if (option != "")
                options.Add(option);

            return options;
        }

        public String FindQuery(String msg)
        {
            bool forFound = false;
            StringBuilder query = new StringBuilder();

            String[] words = msg.Split(" ");
            foreach (String word in words)
            {
                if (word == "for")
                {
                    forFound = true;
                    continue;
                }

                if (forFound)
                {
                    query.Append(word);
                    query.Append(' ');
                }
            }

            if (!forFound)
                return "";
            else
                return query.ToString();
        }

        
    }

}


