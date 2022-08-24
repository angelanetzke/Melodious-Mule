using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace MelodiousMule
{
	internal class Room
	{
		public enum Direction { N, S, W, E };
		private readonly Room[] neighbors = new Room[4];
		private readonly bool[] isConnected = new bool[4];
		private int minX;
		private int maxX;
		private int minY;
		private int maxY;
		private int centerX;
		private int centerY;
		private bool isVisited;
		private readonly Random RNG = new();

		public void SetNeighbor(Room newNeighbor, Direction direction)
		{
			neighbors[(int)direction] = newNeighbor;
			isConnected[(int)direction] = true;
		}

		public Room GetNeighbor(Direction direction)
		{
			return neighbors[(int)direction];
		}

		public bool IsConnected(Direction direction)
		{
			return isConnected[(int)direction];
		}

		public void Reset()
		{
			SetIsVisited(false);
			for (int i = 0; i < isConnected.Length; i++)
			{
				isConnected[i] = false;
			}
		}

		public void SetIsVisited(bool isVisited)
		{
			this.isVisited = isVisited;
		}

		public bool IsVisited()
		{
			return isVisited;
		}

		public Room ConnectToRandom()
		{
			//Prefer to connect to neighbors who haven't been visited
			var indices = new List<int>();
			for (int i = 0; i < isConnected.Length; i++)
			{
				if (!isConnected[i] && neighbors[i] != null && !neighbors[i].IsVisited())
				{
					indices.Add(i);
				}
			}
			if (indices.Count == 0)
			{
				for (int i = 0; i < isConnected.Length; i++)
				{
					if (!isConnected[i] && neighbors[i] != null)
					{
						indices.Add(i);
					}
				}
			}
			if (indices.Count > 0)
			{
				var selectedIndex = indices[RNG.Next(indices.Count)];
				isConnected[selectedIndex] = true;
				var selectedNeighbor = neighbors[selectedIndex];				
				selectedNeighbor.SetIsVisited(true);
				for (int i = 0; i < selectedNeighbor.neighbors.Length; i++)
				{
					if (selectedNeighbor.neighbors[i] == this)
					{
						selectedNeighbor.isConnected[i] = true;
					}
				}
				return selectedNeighbor;
			}
			else
			{
				return null;
			}
		}

		public void SetDimensions(int minX, int maxX, int minY, int maxY, int centerX, int centerY)
		{
			this.minX = minX;
			this.maxX = maxX;
			this.minY = minY;
			this.maxY = maxY;
			this.centerX = centerX;
			this.centerY = centerY;
		}

		public int GetMinX()
		{
			return minX;
		}

		public int GetMaxX()
		{
			return maxX;
		}

		public int GetMinY()
		{
			return minY;
		}

		public int GetMaxY()
		{
			return maxY;
		}

		public int GetCenterX()
		{
			return centerX;
		}

		public int GetCenterY()
		{
			return centerY;
		}

		public Point GetRandomPointInside(int buffer = 0)
		{
			int x = RNG.Next(minX + 1 + buffer, maxX - buffer);
			int y = RNG.Next(minY + 1 + buffer, maxY - buffer);
			return new Point(x, y);
		}

	}
}
