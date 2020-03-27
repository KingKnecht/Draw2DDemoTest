using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Core.Geo;

namespace Draw2D.Core.Utlils
{
    public class SnapCluster<TPayload>
    {
        private readonly double _xSnapSize;
        private readonly double _ySnapSize;
        private readonly Dictionary<int, List<Node<TPayload>>> _xClusters = new Dictionary<int, List<Node<TPayload>>>();
        private readonly Dictionary<int, List<Node<TPayload>>> _yClusters = new Dictionary<int, List<Node<TPayload>>>();
        private readonly Dictionary<TPayload, HashSet<int>> _payloadToClusterMapX = new Dictionary<TPayload, HashSet<int>>();
        private readonly Dictionary<TPayload, HashSet<int>> _payloadToClusterMapY = new Dictionary<TPayload, HashSet<int>>();

        public SnapCluster(double xSnapSize, double ySnapSize)
        {
            _xSnapSize = xSnapSize;
            _ySnapSize = ySnapSize;
        }

        public int Count
        {
            get { return _xClusters.Sum(x => x.Value.Count); }
        }

        public List<Node<TPayload>> GetAllNodes()
        {
            return _xClusters.Values.SelectMany(n => n)
                .Concat(_yClusters.Values.SelectMany(n => n))
                .ToList();
        }

        public void Add(IEnumerable<Point> points, TPayload payload)
        {
            foreach (var point in points)
            {
                Add(point, payload);
            }
        }

        public void Add(Point point, TPayload payload)
        {
            var clusterNoX = (int)(point.X / _xSnapSize);
            var clusterNoY = (int)(point.Y / _ySnapSize);

            InsertToClusterX(point, payload, clusterNoX);
            InsertPayloadToClusterMapX(payload, clusterNoX);

            InsertToClusterY(point, payload, clusterNoY);
            InsertPayloadToClusterMapY(payload, clusterNoY);
        }

        private void InsertToClusterY(Point point, TPayload payload, int clusterNoY)
        {
            List<Node<TPayload>> yCluster;
            if (!_yClusters.TryGetValue(clusterNoY, out yCluster))
            {
                _yClusters[clusterNoY] = new List<Node<TPayload>>();
            }
            _yClusters[clusterNoY].Add(new Node<TPayload>(point.Y, payload, point));
        }

        private void InsertPayloadToClusterMapX(TPayload payload, int clusterNoX)
        {
            HashSet<int> listOfClusterX;
            if (!_payloadToClusterMapX.TryGetValue(payload, out listOfClusterX))
            {
                _payloadToClusterMapX[payload] = new HashSet<int>();
            }
            _payloadToClusterMapX[payload].Add(clusterNoX);
        }

        private void InsertPayloadToClusterMapY(TPayload payload, int clusterNoY)
        {
            HashSet<int> listOfClusterY;
            if (!_payloadToClusterMapY.TryGetValue(payload, out listOfClusterY))
            {
                _payloadToClusterMapY[payload] = new HashSet<int>();
            }
            _payloadToClusterMapY[payload].Add(clusterNoY);
        }

        private void InsertToClusterX(Point point, TPayload payload, int clusterNoX)
        {
            List<Node<TPayload>> xCluster;
            if (!_xClusters.TryGetValue(clusterNoX, out xCluster))
            {
                _xClusters[clusterNoX] = new List<Node<TPayload>>();
            }
            _xClusters[clusterNoX].Add(new Node<TPayload>(point.X, payload, point));
        }

        public Node<TPayload>[] GetXNodes(double xPos, IEnumerable<TPayload> blackList)
        {
            var clusterNoX = (int)(xPos / _xSnapSize);
            List<Node<TPayload>> xCluster;

            var result = new List<Node<TPayload>>();

            if (_xClusters.TryGetValue(clusterNoX - 1, out xCluster))
            {
                result.AddRange(xCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            if (_xClusters.TryGetValue(clusterNoX, out xCluster))
            {
                result.AddRange(xCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            if (_xClusters.TryGetValue(clusterNoX + 1, out xCluster))
            {
                result.AddRange(xCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            return result.Where(n => Math.Abs(xPos - n.Position) <= _xSnapSize).ToArray();
        }

        public Node<TPayload> GetNearestXNode(double xPos, IEnumerable<TPayload> blackList)
        {
            return GetXNodes(xPos, blackList).OrderBy(n => Math.Abs(xPos - n.Position))
              .FirstOrDefault();
        }

        public Node<TPayload>[] GetYNodes(double yPos, IEnumerable<TPayload> blackList)
        {
            var clusterNoY = (int)(yPos / _ySnapSize);

            List<Node<TPayload>> yCluster;

            var result = new List<Node<TPayload>>();

            if (_yClusters.TryGetValue(clusterNoY - 1, out yCluster))
            {
                result.AddRange(yCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            if (_yClusters.TryGetValue(clusterNoY, out yCluster))
            {
                result.AddRange(yCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            if (_yClusters.TryGetValue(clusterNoY + 1, out yCluster))
            {
                result.AddRange(yCluster.Where(n => !blackList.Contains(n.Payload)));
            }

            return result.Where(n => Math.Abs(yPos - n.Position) <= _xSnapSize).ToArray();
        }

        public Node<TPayload> GetNearestYNode(double yPos, IEnumerable<TPayload> blackList)
        {
            return GetYNodes(yPos, blackList).OrderBy(n => Math.Abs(yPos - n.Position))
               .FirstOrDefault();
        }

        public void Remove(TPayload payload)
        {
            HashSet<int> xClusterList;
            if (_payloadToClusterMapX.TryGetValue(payload, out xClusterList))
            {
                foreach (var clusterNo in xClusterList)
                {
                    _xClusters[clusterNo].RemoveAll(n => n.Payload.Equals(payload));
                    //Clean up empty clusters
                    if (_xClusters[clusterNo].Count == 0)
                    {
                        _xClusters.Remove(clusterNo);
                    }
                    //Clean up payload map
                    _payloadToClusterMapX.Remove(payload);
                }
            }

            HashSet<int> yClusterList;
            if (_payloadToClusterMapY.TryGetValue(payload, out yClusterList))
            {
                foreach (var clusterNo in yClusterList)
                {
                    _yClusters[clusterNo].RemoveAll(n => n.Payload.Equals(payload));

                    //Clean up empty clusters
                    if (_yClusters[clusterNo].Count == 0)
                    {
                        _yClusters.Remove(clusterNo);
                    }
                    //Clean up payload map
                    _payloadToClusterMapY.Remove(payload);

                }
            }

            //Console.WriteLine("XCluster Count: " +_xClusters.Count);
            //Console.WriteLine("YCluster Count: " + _yClusters.Count);
        }
    }
}