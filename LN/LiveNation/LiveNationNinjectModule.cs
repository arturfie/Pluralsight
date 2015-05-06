using LiveNation.Services;
using LiveNation.Services.Interfaces;
using Ninject.Modules;

namespace LiveNation
{
    public class LiveNationNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISolution>().To<LiveNationSolution>();
            Bind<IDisplay>().To<ConsoleDisplay>();
        }
    }
}
