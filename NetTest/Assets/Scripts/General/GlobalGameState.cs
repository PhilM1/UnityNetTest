using UnityEngine;
using System.Collections;

public enum ConnectionType
{
	Client,
	Host,
	Dedicated
}

public class GlobalGameState
{
	
	private static GlobalGameState m_Instance;


	public string ipAddress = string.Empty;
	public int port = 25001;
	public ConnectionType connectionType; //set through main menu
	public int maxPlayers = 32;

	public GlobalGameState()
	{

	}


	public static GlobalGameState Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = new GlobalGameState();
			}
			
			return m_Instance;
		}
	}
}