using RouteCalculator.Models;

namespace RouteCalculator.Services
{
    public class MapService : IMapService
    {
        private GraphDto _currentMap;

        public void SetMap (GraphDto graph)
        {
            _currentMap = graph;
        }

        public GraphDto GetMap()
        {
            return _currentMap;
        }
    }
}
