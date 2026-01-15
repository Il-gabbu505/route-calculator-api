using RouteCalculator.Models;

namespace RouteCalculator.Services
{
    public interface IMapService
    {
        void SetMap(GraphDto graph);
        GraphDto GetMap();
    }
}
