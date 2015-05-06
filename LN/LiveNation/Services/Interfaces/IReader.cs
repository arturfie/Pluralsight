using System.Collections.Generic;

namespace LiveNation.Services.Interfaces
{
    interface IReader
    {
        IEnumerable<int> GetInputs();
        IDictionary<int, string> GetRules();
    }
}
