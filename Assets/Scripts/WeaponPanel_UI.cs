using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel_UI : MonoBehaviour
{

		public Text WeaponName;
		public Text WeaponDamage;
		public Image WeaponIcon;
		public Image WeaponDamageTypeIcon;

		// Use this for initialization
		void Start ()
		{
		
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}

		/// <summary>
		/// Populate panel with information from <paramref name="wep"/>
		/// </summary>
		/// <param name="wep">Wep.</param>
		public void SetCurrentDisplayWeapon (Weapon wep)
		{
				SetWeaponName (wep.DisplayName);
				SetWeaponDamage (wep.MinimumDamage, wep.MaximumDamage); 
				SetWeaponIcon (wep.Icon);
				SetWeaponDamageTypeIcon (wep.DamageTypeIcon);
		}

		public void SetWeaponName (string name)
		{
				if (WeaponName == null)
						return;
				
				WeaponName.text = String.Format ("Weapon: {0}", name);
		}

		public void SetWeaponDamage (int low, int high)
		{
				if (WeaponDamage == null)
						return;
			
				WeaponDamage.text = String.Format ("Damage: ({0} - {1})", low, high);
		}

		public void SetWeaponIcon (Sprite icon)
		{
				if (WeaponIcon == null)
						return;
				
				WeaponIcon.sprite = icon;
		}

		public void SetWeaponDamageTypeIcon (Sprite icon)
		{
				if (WeaponDamageTypeIcon == null)
						return;
				
				WeaponDamageTypeIcon.sprite = icon;
		}

}
