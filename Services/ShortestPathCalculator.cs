using RouteCalculator.Models;
namespace RouteCalculator.Services
{
    public static class ShortestPathCalculator
    {
        public static string GetPath(GraphDto graph, string start, string end)
        {
            var (dist, prev) = Dijkstra(graph, start);
            var path = new Stack<string>();
            var current = end;

            while (current != null)
            {
                path.Push(current);
                prev.TryGetValue(current, out current);
            }

            return string.Join("", path);
        }

        public static int GetDistance(GraphDto graph, string start, string end)
        {
            var (dist, _) = Dijkstra(graph, start);
            return dist[end];
        }

        private static (Dictionary<string, int>, Dictionary<string, string>)
            Dijkstra(GraphDto graph, string start)
        {
            var distances = graph.Nodes.ToDictionary(n => n.NodeId, _ => int.MaxValue);
            var previous = new Dictionary<string, string>();
            distances[start] = 0;

            var edges = graph.Edges;

            var nodes = new HashSet<string>(distances.Keys);

            while (nodes.Count > 0)
            {
                var current = nodes.OrderBy(n => distances[n]).First();
                nodes.Remove(current);

                foreach (var edge in edges.Where(e => e.From == current || e.To == current))
                {
                    var neighbour = edge.From == current ? edge.To : edge.From;
                    var alt = distances[current] + edge.Distance;

                    if (alt < distances[neighbour])
                    {
                        distances[neighbour] = alt;
                        previous[neighbour] = current;
                    }
                }
            }

            return (distances, previous);
        }
    }
}
