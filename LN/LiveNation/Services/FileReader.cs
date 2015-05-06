using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using LiveNation.Services.Interfaces;

namespace LiveNation.Services
{
    class FileReader : IReader
    {
        public IEnumerable<int> GetInputs()
        {
            string pathToInputFile = ConfigurationManager.AppSettings["inputFilePath"];
            IEnumerable<int> inputs = new List<int>();

            try
            {
                inputs = File.ReadAllText(pathToInputFile).Split(' ').Select(int.Parse);
            }
            catch (FormatException exception)
            {
                //logger?
            }
            catch (Exception)
            {
                throw;
            }

            return inputs;
        }

        public IDictionary<int, string> GetRules()
        {
            string pathToRulesFile = ConfigurationManager.AppSettings["rulesFilePath"];
            IDictionary<int, string> rulesDictionary = new Dictionary<int, string>();
            
            foreach (var line in File.ReadAllLines(pathToRulesFile))
            {
                try
                {
                    var splitedArray = line.Split(':');
                    rulesDictionary.Add(int.Parse(splitedArray[0]), splitedArray[1]);
                }
                catch (FormatException exception)
                {
                    //logger?
                }
                catch (ArgumentException exception)
                {
                    //logger?
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }

            return rulesDictionary;
        }
    }
}
