using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class GameOver : CanvasLayer
{
	
	private void OnNewGameButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
	}
}



