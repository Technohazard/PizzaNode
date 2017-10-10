using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel_UI : MonoBehaviour {

		public Text PlayerName;
		public Text PlayerHealth;
		public Text PlayerShield;
		public Text PlayerMoney;
		public Text PlayerLevel;

		/// <summary>
		/// Player that owns the information on this panel.
		/// </summary>
		public Player PanelOwner;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

		public void SetCurrentDisplayPlayer(Player player)
		{
			PanelOwner = player;
			
			SetPlayerName(player.Name);
			SetPlayerHealth (player.Health);
			SetPlayerShield (player.Shield);
			SetPlayerMoney (player.Money);
			SetPlayerLevel (player.Level);
		}

		public void SetPlayerHealth (double hp, double hpmax = 9999)
		{
			string msg = "HP: {0}, {1}";
				PlayerHealth.text = string.Format(msg, (int)hp, (int)hpmax);
		}

		public void SetPlayerName(string name)
		{
			string msg = "{0}";
				PlayerName.text = string.Format (msg, name);
		}

		public void SetPlayerShield(double shield, double shieldMax = 9999)
		{
			string msg = "(S): {0} / {1}";
				PlayerShield.text = string.Format (msg, (int)shield, (int)shieldMax);
		}

		public void SetPlayerMoney(int money)
		{
			string msg = "$: {0}";
				PlayerMoney.text = string.Format (msg, money);
		}

		public void SetPlayerLevel(int level)
		{
			string msg = "LVL: {0}";
				PlayerLevel.text = string.Format (msg, level);
		}
}
