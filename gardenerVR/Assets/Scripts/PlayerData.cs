using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerData
{
	public int plantLevel;
	public string plantStage;

	public PlayerData(Player player)
	{
		plantLevel = player.plantLevel;
		plantStage = player.plantStage;
	}
}
