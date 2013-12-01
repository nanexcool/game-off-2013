using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletHell.Engine
{
    class SearchNode
    {
        public Point Position { get; set; }
        public bool Walkable { get; set; }
        public SearchNode[] Neighbors { get; set; }
        public SearchNode Parent { get; set; }
        public bool InOpenList { get; set; }
        public bool InClosedList { get; set; }
        public float DistanceToGoal { get; set; }
        public float DistanceTraveled { get; set; }
    }

    public class Pathfinder
    {
        private SearchNode[] searchNodes;

        private List<SearchNode> openList = new List<SearchNode>();
        private List<SearchNode> closedList = new List<SearchNode>();

        //public Level Level { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Pathfinder(Level level)
        {
            //Level = level;

            Width = level.Width;
            Height = level.Height;

            InitializeSearchNodes(level);
        }

        private void InitializeSearchNodes(Level level)
        {
            searchNodes = new SearchNode[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SearchNode node = new SearchNode();
                    node.Position = new Point(x, y);
                    node.Walkable = !level.GetTile(x, y).IsSolid();
                    if (node.Walkable)
                    {
                        node.Neighbors = new SearchNode[4];
                        searchNodes[x + y * Width] = node;
                    }
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SearchNode node = searchNodes[x + y * Width];

                    if (node == null || node.Walkable == false)
                    {
                        continue;
                    }

                    Point[] neighbors = new Point[] 
                    {
                        new Point(x, y - 1),
                        new Point(x, y + 1),
                        new Point(x - 1, y),
                        new Point(x + 1, y)
                    };

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        Point position = neighbors[i];

                        if (position.X < 0 || position.X >= Width || position.Y < 0 || position.Y >= Height)
                        {
                            continue;
                        }

                        SearchNode neighbor = searchNodes[position.X + position.Y * Width];

                        if (neighbor == null || neighbor.Walkable == false)
                        {
                            continue;
                        }

                        node.Neighbors[i] = neighbor;
                    }
                }
            }
        }

        private float Heuristic(Point point1, Point point2)
        {
            return Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);
        }

        private void ResetSearchNodes()
        {
            openList.Clear();
            closedList.Clear();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SearchNode node = searchNodes[x + y * Width];

                    if (node == null)
                    {
                        continue;
                    }

                    node.InOpenList = false;
                    node.InClosedList = false;

                    node.DistanceTraveled = float.MaxValue;
                    node.DistanceToGoal = float.MaxValue;
                }
            }
        }

        private SearchNode FindBestNode()
        {
            SearchNode currentTile = openList[0];

            float smallestDistanceToGoal = float.MaxValue;

            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].DistanceToGoal < smallestDistanceToGoal)
                {
                    currentTile = openList[i];
                    smallestDistanceToGoal = currentTile.DistanceToGoal;
                }
            }

            return currentTile;
        }

        private List<Vector2> FindFinalPath(SearchNode startNode, SearchNode endNode)
        {
            closedList.Add(endNode);

            SearchNode parentTile = endNode.Parent;

            while (parentTile != startNode)
            {
                closedList.Add(parentTile);
                parentTile = parentTile.Parent;
            }

            List<Vector2> finalPath = new List<Vector2>();

            for (int i = closedList.Count - 1; i >= 0; i--)
            {
                finalPath.Add(new Vector2(closedList[i].Position.X * Tile.Size, closedList[i].Position.Y * Tile.Size));
            }

            return finalPath;
        }

        public List<Vector2> FindPath(Point startPoint, Point endPoint)
        {
            if (startPoint == endPoint)
            {
                return new List<Vector2>();
            }

            ResetSearchNodes();

            SearchNode startNode = searchNodes[startPoint.X + startPoint.Y * Width];
            SearchNode endNode = searchNodes[endPoint.X + endPoint.Y * Width];

            if (startNode == null || endNode == null)
            {
                return new List<Vector2>();
            }

            startNode.InOpenList = true;

            startNode.DistanceToGoal = Heuristic(startPoint, endPoint);
            startNode.DistanceTraveled = 0;

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                SearchNode currentNode = FindBestNode();

                if (currentNode == null)
                {
                    break;
                }

                if (currentNode == endNode)
                {
                    return FindFinalPath(startNode, endNode);
                }

                for (int i = 0; i < currentNode.Neighbors.Length; i++)
                {
                    SearchNode neighbor = currentNode.Neighbors[i];

                    if (neighbor == null || neighbor.Walkable == false)
                    {
                        continue;
                    }

                    float distanceTraveled = currentNode.DistanceTraveled + 1;

                    float heuristic = Heuristic(neighbor.Position, endPoint);

                    if (neighbor.InOpenList == false && neighbor.InClosedList == false)
                    {
                        neighbor.DistanceTraveled = distanceTraveled;

                        neighbor.DistanceToGoal = distanceTraveled + heuristic;

                        neighbor.Parent = currentNode;

                        neighbor.InOpenList = true;

                        openList.Add(neighbor);
                    }
                    else if (neighbor.InOpenList || neighbor.InClosedList)
                    {
                        if (neighbor.DistanceTraveled > distanceTraveled)
                        {
                            neighbor.DistanceTraveled = distanceTraveled;
                            neighbor.DistanceToGoal = distanceTraveled + heuristic;

                            neighbor.Parent = currentNode;
                        }
                    }
                }

                openList.Remove(currentNode);
                currentNode.InClosedList = true;
            }

            return new List<Vector2>();
        }
    }
}
