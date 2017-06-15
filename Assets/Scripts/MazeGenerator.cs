using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Point
{
	public int x;
	public int y;
	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

[System.Serializable]
public class MazeNode
{
	Point position;
	List<MazeNode> neighbors = new List<MazeNode>();
	List<MazeNode> connectedNode = new List<MazeNode>();
	bool visited = false;

	MazeNode parent = null;
	public MazeNode(Point position)
	{
		this.position = position;
	}
	public Point GetPosition()
	{
		return position;
	}
	public void AddNeighbor(MazeNode neighbor)
	{
		neighbors.Add (neighbor);
	}
	public void AddConnectedNode(MazeNode node)
	{
		connectedNode.Add (node);
	}
	public bool CheckConnectNode(MazeNode node)
	{
		return connectedNode.Contains (node);
	}
	public List<MazeNode> GetNeighbors()
	{
		return neighbors;
	}
	public bool WasVisited()
	{
		return visited;
	}
	public void Visit()
	{
		visited = true;
	}
	public void SetParent(MazeNode node)
	{
		parent = node;
	}
	public MazeNode GetParent()
	{
		return parent;
	}

}

public class MazeGenerator : MonoBehaviour {
	[SerializeField] int size_x;
	[SerializeField] int size_y;
	[SerializeField] float tileSize = 2.0f;


	[SerializeField] GameObject wallPrefab;
	[SerializeField]
	List<MazeNode> debugNodes = new List<MazeNode>();
	Dictionary<Point, MazeNode> nodes = new Dictionary<Point, MazeNode>();

	System.DateTime begin;
	// Use this for initialization
	void Start () {
		begin = System.DateTime.Now;
		//Create the nodes
		for (int x = 0; x < size_x; x++) 
		{
			for (int y = 0; y < size_y; y++) 
			{
				Point point = new Point (x, y);
				MazeNode node = new MazeNode (point);
				nodes [point] = node;
				debugNodes.Add (node);
			}
		}
		//Connect neighbors
		foreach (var node in nodes.Values) 
		{
			for (int dx = -1; dx <= 1; dx++) 
			{
				for (int dy = -1; dy <= 1; dy++) 
				{
					if (dx == dy || dx == -dy)
						continue;
					Point neighborP = new Point (node.GetPosition ().x + dx, node.GetPosition ().y + dy);
					if (neighborP.x == -1 || neighborP.x == size_x||
						neighborP.y == -1 || neighborP.y == size_y)
						continue;
					MazeNode neighbor = nodes [neighborP];
					node.AddNeighbor(neighbor);
				}
			}
		}
		Debug.Log ("Node generation: " + (System.DateTime.Now - begin).TotalSeconds);
		BFSGenerateMaze ();
		Debug.Log ("Maze generation: " + (System.DateTime.Now - begin).TotalSeconds);
		InstantiateMaze ();
		Debug.Log ("Total Time: " + (System.DateTime.Now - begin).TotalSeconds);
	}
	void BFSGenerateMaze()
	{
		//BFS
		MazeNode currentNode = nodes[new Point(0,0)];
		currentNode.Visit ();
		List<MazeNode> previousLayer = new List<MazeNode> ();
		List<MazeNode> nextLayer = new List<MazeNode> ();
		while (currentNode != null) 
		{

			foreach (var neighbor in currentNode.GetNeighbors()) 
			{
				if (!neighbor.WasVisited ()) 
				{
					neighbor.Visit ();
					neighbor.SetParent (currentNode);
					nextLayer.Add (neighbor);
					neighbor.AddConnectedNode (currentNode);
					currentNode.AddConnectedNode (neighbor);
				}
			}
			if (previousLayer.Count == 0) {
				if (nextLayer.Count == 0) {
					currentNode = null;
					break;
				}
				foreach (var node in nextLayer) 
				{
					previousLayer.Add (node);
				}
				nextLayer.Clear ();
				int random_index = Random.Range (0, previousLayer.Count);
				currentNode = previousLayer [random_index];
				previousLayer.RemoveAt (random_index);
			} 
			else 
			{
				int random_index = Random.Range (0, previousLayer.Count);
				currentNode = previousLayer [random_index];
				previousLayer.RemoveAt (random_index);
			}
		}
	}
	void DFSGenerateMaze()
	{
		//DFS
		MazeNode currentNode = nodes[new Point(0,0)];
		currentNode.Visit ();
		while (currentNode != null) 
		{
			
			List<MazeNode> tmpNodes = new List<MazeNode> ();
			foreach (var neighbor in currentNode.GetNeighbors()) 
			{
				if (!neighbor.WasVisited ()) 
				{
					tmpNodes.Add (neighbor);
				}
			}
			if (tmpNodes.Count == 0) {
				if (currentNode.GetParent () == null)
					currentNode = null;
				else
					currentNode = currentNode.GetParent ();
			} 
			else 
			{
				MazeNode nextNode = tmpNodes [Random.Range (0, tmpNodes.Count)];
				nextNode.Visit ();
				nextNode.SetParent (currentNode);
				currentNode.AddConnectedNode (nextNode);
				nextNode.AddConnectedNode (currentNode);
				currentNode = nextNode;
			}
		}
	}
	void InstantiateWall(Vector3 position)
	{
		GameObject wall = Instantiate (wallPrefab);
		wall.transform.position = position;
	}
	void InstantiateMaze()
	{
		//Between walls
		for (int x = 0; x <= size_x; x++) 
		{
			for (int y = 0; y <= size_y; y++) 
			{
				
				InstantiateWall( new Vector3 (
					tileSize*(x-size_x/2-0.5f), tileSize*(y-size_y/2-0.5f)));
			}
		}
		//Horizontal walls
		for (int x = 0; x < size_x; x++) 
		{
			for (int y = 0; y <= size_y; y++) 
			{
				

				if (y == 0 || y == size_y) {
					InstantiateWall (new Vector3 (
						tileSize * (x - size_x / 2), tileSize * (y - size_y / 2 - 0.5f)));
				} 
				else 
				{
					MazeNode currentNode = nodes [new Point (x, y - 1)];
					if(!currentNode.CheckConnectNode(nodes[new Point(x,y)]))
					{
						InstantiateWall (new Vector3 (
							tileSize * (x - size_x / 2), tileSize * (y - size_y / 2 - 0.5f)));
					}
				}
				
			}
		}
		//Vertical walls
		for (int x = 0; x <= size_x; x++) 
		{
			for (int y = 0; y < size_y; y++) 
			{
				if (x == 0 || x == size_y) {
					
					InstantiateWall (new Vector3 (
						tileSize * (x - size_x / 2 - 0.5f), tileSize * (y - size_y / 2)));
				} else {
					MazeNode currentNode = nodes [new Point (x-1, y)];
					if (!currentNode.CheckConnectNode (nodes [new Point (x, y)])) {
						InstantiateWall (new Vector3 (
							tileSize * (x - size_x / 2 - 0.5f), tileSize * (y - size_y / 2)));
					}
				}

			}
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
