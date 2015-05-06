using System;
using System.Collections.Generic;
using System.Configuration;
using LiveNation.Services.Interfaces;

namespace LiveNation.Services
{
    class NumberDivider : INumberDivider
    {
        private IDisplay _display;
        private IReader _reader;

        public NumberDivider(IDisplay display, IReader reader)
        {
            _display = display;
            _reader = reader;
        }

        public void DisplayDivisionResult(int number)
        {
            var inputs = _reader.GetInputs();
            var rules = _reader.GetRules();

            foreach (var inputNumber in inputs)
            {
                string message = RunNumberThroughRules(rules, inputNumber);
                _display.Write(message + " ");
            }
        }

        private string RunNumberThroughRules(IDictionary<int, string> rules, int inputNumber)
        {
            string message = String.Empty;

            foreach (var rule in rules)
            {
                if (inputNumber % rule.Key == 0)
                {
                    message += rule.Value;
                }
            }
            if (String.IsNullOrEmpty(message))
            {
                message = inputNumber.ToString();
            }
            return message;
        }

        public void DisplayOccurenciesReport(int number)
        {
            throw new NotImplementedException();
        }
    }
}
