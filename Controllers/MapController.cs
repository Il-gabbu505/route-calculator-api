using Microsoft.AspNetCore.Mvc;
using RouteCalculator.Models;
using RouteCalculator.Services;

namespace RouteCalculator.Controllers
{
    [ApiController]
    [Route("api/map")]
    public class MapController : ControllerBase
    {
        private readonly IMapService _mapService;


        private bool IsAuthorized(string apiKey, bool requiresWrite)
        {
            if (string.IsNullOrEmpty(apiKey))
                return false;

            if (requiresWrite && apiKey != "FS_ReadWrite")
                return false;

            if (!requiresWrite && apiKey != "FS_Read")
                return false;

            return true;
        }

        public MapController(IMapService mapService)
        {
            _mapService = mapService;
        }

        [HttpPost("SetMap")]
        public IActionResult SetMap([FromHeader(Name ="X-Api-Key")]  string apiKey,[FromBody] GraphDto graph)
        {
            if (!IsAuthorized(apiKey, requiresWrite: true))
                return Unauthorized("Invalid or missing API key.");

            if (graph == null || graph.Nodes == null || graph.Edges == null)
                return BadRequest("Map data is missing or invalid.");

            _mapService.SetMap(graph);
            return Ok();
        }

        [HttpGet("GetMap")]
        public IActionResult GetMap([FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            if(!IsAuthorized(apiKey, requiresWrite: false))
                    return Unauthorized("Invalid or missing API key.");
            var map = _mapService.GetMap();

            if (map == null)
                return BadRequest("Map is not set");
            return Ok(map);
        }

        [HttpGet("ShortestRoute")]
        public IActionResult ShortestRoute([FromHeader(Name = "X-Api-Key")]string apiKey,string from, string to)
        {
            if (!IsAuthorized(apiKey, requiresWrite: false))
                return Unauthorized("Invalid or missing API key");

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return BadRequest("Missing from or to parameters.");

            var map = _mapService.GetMap();

            if (map == null)
                return BadRequest("Map not set");
            if (!map.Nodes.Any(n => n.NodeId == from) || !map.Nodes.Any(n => n.NodeId == to))
                return BadRequest("Unknown node name.");

            var path = ShortestPathCalculator.GetPath(map, from, to);
            return Ok(path);
        }

        [HttpGet("ShortestDistance")]
        public IActionResult ShortestDistance([FromHeader(Name = "X-Api-Key")] string apiKey, string from, string to)
        {
            if (!IsAuthorized(apiKey, requiresWrite: false))
                return Unauthorized("Invalid or missing API key.");

            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                return BadRequest("Missing 'from' or 'to' parameter.");

            var map = _mapService.GetMap();

            if (map == null)
                return BadRequest("Map has not been set.");

            if (!map.Nodes.Any(n => n.NodeId == from) || !map.Nodes.Any(n => n.NodeId == to))
                return BadRequest("Unknown node name.");


            var distance = ShortestPathCalculator.GetDistance(_mapService.GetMap(), from, to);
            return Ok(distance);
        }
    }
}
