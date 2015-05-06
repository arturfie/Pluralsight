using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveNation.Services.Interfaces;

namespace LiveNation.Services
{
    class LiveNationSolution : ISolution
    {
        private INumberDivider _numberDivider;
        public LiveNationSolution(INumberDivider numberDivider)
        {
            _numberDivider = numberDivider;
        }

        public void ExecuteSolution()
        {
            _numberDivider.DisplayDivisionResult();
        }
    }
}
