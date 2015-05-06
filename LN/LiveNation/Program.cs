using LiveNation.Services.Interfaces;
using Ninject;

namespace LiveNation
{
    class Program
    {
        static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new LiveNationNinjectModule());
            ISolution solution = kernel.Get<ISolution>();
            solution.ExecuteSolution();
        }
    }
}
