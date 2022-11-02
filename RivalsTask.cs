using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rivals
{
    public class RivalsTask
    {
        private static readonly Point[] Offsets = {new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)};

        public static IEnumerable<OwnedLocation> AssignOwners(Map map)
        {
            var visited = new HashSet<Point>();
            var queue = new Queue<OwnedLocation>();

            for (var i = 0; i < map.Players.Length; i++)
            {
                visited.Add(map.Players[i]);
                var startOwnedLocation = new OwnedLocation(i, map.Players[i], 0);
                queue.Enqueue(startOwnedLocation);
                yield return startOwnedLocation;
            }

            while (queue.Count != 0)
            {
                var currentOwnedLocation = queue.Dequeue();
                var nextPoints = GetPossibleNextPoint(map, visited, currentOwnedLocation);
                if (nextPoints == null)
                    continue;
                foreach (var nextPoint in nextPoints)
                {
                    var nextOwnedLocation = new OwnedLocation(currentOwnedLocation.Owner, nextPoint,
                        currentOwnedLocation.Distance + 1);
                    queue.Enqueue(nextOwnedLocation);
                    visited.Add(nextPoint);
                    yield return nextOwnedLocation;
                }
            }
        }

        private static List<Point> GetPossibleNextPoint(Map map, HashSet<Point> visited,
            OwnedLocation currentOwnedLocation)
        {
            var lastPoint = currentOwnedLocation.Location;
            var nextPoints = Offsets
                .Where(offset => !visited.Contains(new Point(offset.X + lastPoint.X, offset.Y + lastPoint.Y))
                                 && map.InBounds(offset + (Size) lastPoint)
                                 && map.Maze[(offset + (Size) lastPoint).X, (offset + (Size) lastPoint).Y] !=
                                 MapCell.Wall)
                .Select(p => p + (Size) lastPoint)
                .ToList();
            return nextPoints;
        }
    }
}