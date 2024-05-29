using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;

public partial class main : Node
{
	// TileMap settings
	TileMap tileMap;
	const int SnakeSourceID = 2;
	const int BerrySourceID = 3;
	int cellCount = 20;
	int cellSize = 50; // in pixels	

	// Snake settings
	List<Vector2I> snakeBody;
	Vector2I currMovement; // Initial movement to the right
	
	// Berry settings
	Vector2I berry;
	bool berryEaten;
	
	// Game	settings
	int score;
	PackedScene gameOverScene;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get tilemap node
		tileMap = GetNode<TileMap>("TileMap");
		// create snake body with initial length of 3
		snakeBody = new List<Vector2I>();
		// Fill snake body with initial values
		ResetSnake();
		
		SetBerryLocation();
		berryEaten = false;
		
		gameOverScene = GD.Load<PackedScene>("res://scenes/GameOver.tscn");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CheckGameOver();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("move_up") && currMovement != new Vector2I(0, 1))
		{
			currMovement = new Vector2I(0, -1);
		} 
		else if (@event.IsActionPressed("move_down") && currMovement != new Vector2I(0, -1))
		{
			currMovement = new Vector2I(0, 1);
		}
		else if (@event.IsActionPressed("move_right") && currMovement != new Vector2I(-1, 0))
		{
			currMovement = new Vector2I(1, 0);
		}
		else if (@event.IsActionPressed("move_left") && currMovement != new Vector2I(1, 0))
		{
			currMovement = new Vector2I(-1, 0);
		}
	}

	private void OnTimerTimeout()
	{
		MoveSnake();
		DrawBerry();
		DrawSnake();
		CheckBerryEaten();
	}

	void MoveSnake()
	{
		Console.WriteLine(tileMap.GetUsedCellsById(0, SnakeSourceID));
		if (berryEaten)
		{
			// Enlarge the snake
			DeleteTiles(SnakeSourceID);
			var newHead = snakeBody[0] + currMovement;
			snakeBody.Insert(0, newHead);
			berryEaten = false;
		}
		else
		{
			DeleteTiles(SnakeSourceID);
			var newHead = snakeBody[0] + currMovement;
			snakeBody.Insert(0, newHead);
			snakeBody.RemoveAt(snakeBody.Count - 1);
		}
	}

	void DeleteTiles(int sourceId)
	{
		var tiles = tileMap.GetUsedCellsById(0, sourceId);	
		foreach (var tile in tiles)
		{
			tileMap.SetCell(0, tile, -1, new Vector2I(-1, -1)); // Erases the cell
		}
	}

	void DrawSnake()
	{
		foreach (var bodyPart in snakeBody)
		{
			tileMap.SetCell(0, bodyPart, 2, new Vector2I(0, 0));
		}
	}


	public void ResetSnake()
	{
		snakeBody.Clear();
		var head = new Vector2I(9, 9);
		var body1 = new Vector2I(8, 9);
		var body2 = new Vector2I(7, 9);
		
		snakeBody.Add(head);
		snakeBody.Add(body1);
		snakeBody.Add(body2);

		currMovement = new Vector2I(1, 0);
	}

	void SetBerryLocation()
	{
		var rand = new Random();
		var X = rand.Next() % cellCount;
		var Y = rand.Next() % cellCount;
		berry = new Vector2I(X, Y);
	}
	
	void DrawBerry()
	{
		tileMap.SetCell(0, berry, BerrySourceID, new Vector2I(0, 0));	
	}

	void CheckBerryEaten()
	{
		if (snakeBody[0] == berry)
		{
			berryEaten = true;
			SetBerryLocation();
			score++;
			GetTree().CallGroup("ScoreGroup", "UpdateScore", score);
		}
	}
	
	public void CheckGameOver()
	{
		var head = snakeBody[0];
		if (head.X < 0 || head.X > 19 || head.Y < 0 ||
			head.Y > 19)
		{
			GameOver();
		}

		foreach (var block in snakeBody.GetRange(1, snakeBody.Count - 1))
		{
			if (head == block)
			{
				GameOver();	
			}
		}
	}

	Array<Vector2I> GenerateTileMapCoords()
	{
		var list = new Array<Vector2I>();
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < cellCount; j++)
			{
				var coords = new Vector2I(i, j);
				list.Add(coords);
			}
		}

		return list;
	}
	
	
	void GameOver()
	{
		var instance = gameOverScene.Instantiate();
		AddChild(instance);
		
		// stop timer
		GetNode<Timer>("Timer").Stop();
	}
	
}

