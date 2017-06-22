using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Chronometer
{
	public float period;
	public float time;

	public Chronometer(float initialTime, float constPeriod)
	{
		period = constPeriod;
		time = initialTime;
	}
	public void Update(float deltaTime)
	{
		if(time >= 0)
			time -= deltaTime;

	}
	public bool Over()
	{
		return time <= 0;
	}
	public void SetPeriod(float newPeriod)
	{
		if(newPeriod < time)
		{
			time = -1.0f;
		}
		period = newPeriod;
	}
	public void Reset()
	{
		time = period;
	}
	public float Current()
	{
		return (period - time) / period;
	}
	public float CurrentTime()
	{
		return period - time;
	}
	public float Remain()
	{
		return time;
	}
}

public class CellularAutomaton : MonoBehaviour {
	public int sizeX = 5;
	public int sizeY = 5;

	public GameObject cellulePrefab;

	public Dictionary<Vector2, GameObject> cellules = new Dictionary<Vector2, GameObject> ();

	public float celluleInitRepartition = 0.4f;

	Chronometer stepTimer = new Chronometer(5.0f, 1.0f);
	public int stepNumber = 10;
	// Use this for initialization
	void Start () {
		GenerateRandomInit ();
	}

	GameObject CreateCellule(Vector2 pos)
	{
		GameObject cellule = Instantiate (cellulePrefab);
		cellule.transform.position = pos;
		cellule.transform.parent = transform;
		return cellule;
	}

	void GenerateRandomInit()
	{
		for (int i = -sizeX; i <= sizeX; i++) 
		{
			for (int j = -sizeY; j <= sizeY; j++) 
			{
				Vector2 pos = new Vector2 (i, j);
				if (Random.Range (0.0f, 1.0f) < celluleInitRepartition) 
				{
					
					cellules [pos] = CreateCellule(pos);
				}
			}
		}
	}

	void CellularStep()
	{
		List<Vector2> toDestroy = new List<Vector2>();
		List<Vector2> toCreate = new List<Vector2>();

		for (int i = -sizeX; i <= sizeX; i++) 
		{
			for (int j = -sizeY; j <= sizeY; j++) 
			{
				Vector2 pos = new Vector2 (i, j);
				int neighborNmb = 0;
				//check the number of neighbor
				for (int dx = -1; dx <= 1; dx++) 
				{
					for (int dy = -1; dy <= 1; dy++) 
					{
						if (dx == 0 && dy == 0)
							continue;
						Vector2 neighborPos = pos + new Vector2 (dx, dy);
						if (cellules.ContainsKey (neighborPos))
							neighborNmb++;
					}
				}
				//Debug.Log (neighborNmb);
				if (cellules.ContainsKey (pos))
                {
					if(neighborNmb<4)
                    { 
						toDestroy.Add (pos);
						
					
					}
				} else
                {
					if(neighborNmb>4)
                    { 
						toCreate.Add (pos);
					}
				}
			}
		}
		foreach (Vector2 destroyedPos in toDestroy) 
		{
			Destroy (cellules [destroyedPos]);
			cellules.Remove (destroyedPos);
		}
		foreach (Vector2 createdPos in toCreate) 
		{
			cellules [createdPos] = CreateCellule (createdPos);
		}
		stepNumber--;
	}

	// Update is called once per frame
	void Update () {
		stepTimer.Update (Time.deltaTime);
		if (stepTimer.Over () && stepNumber >= 0) 
		{
			CellularStep ();
			stepTimer.Reset ();
		}
	}
}
