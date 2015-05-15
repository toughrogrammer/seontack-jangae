﻿using UnityEngine;
using System.Collections;

public class SceneControllerBattle : MonoBehaviour {

	public Transform GameObjectPlayer;
	public Transform GameObjectMonster;
	public Transform TransformResultPanel;
	public UISprite SpriteWin;
	public UISprite SpriteLose;
	public float ResultPanelTransitionTime;
	BattleCharacter _battleCharacterPlayer;
	BattleCharacter _battleCharacterMonster;

	void Start() {
		// for debugging
		PlayerPrefs.SetInt (PreferenceKeys.KEY_CURRENT_STAGE, 1);

		_battleCharacterPlayer = new BattleCharacter (
			new AbilityData(VariableStorage.Instance.PlayerStats1, 
		                VariableStorage.Instance.PlayerStats2, 
		                VariableStorage.Instance.PlayerStats3)
			,
			Leveling.GetMonsterDataByStage(
				PlayerPrefs.GetInt (PreferenceKeys.KEY_CURRENT_STAGE, 1))
			);
		_battleCharacterMonster = new BattleCharacter(
			Leveling.GetMonsterDataByStage2(
				PlayerPrefs.GetInt (PreferenceKeys.KEY_CURRENT_STAGE, 1))
			);
	}

	public void OnCollisionBetweenBattleCharacter() {
		print ("damage!");

		float calculatedDamage1 = _battleCharacterPlayer.CalculateDamage (_battleCharacterMonster);
		float calculatedDamage2 = _battleCharacterMonster.CalculateDamage (_battleCharacterPlayer);

		_battleCharacterPlayer.HP -= calculatedDamage2;
		_battleCharacterMonster.HP -= calculatedDamage1;

		Debug.Log (_battleCharacterPlayer.HP + " / " + _battleCharacterMonster.HP);
		if (_battleCharacterPlayer.HP <= 0) {
			OnLose();
		} else if (_battleCharacterMonster.HP <= 0) {
			OnWin();
		}
	}

	public void OnWin() {
		GameObjectPlayer.gameObject.SetActive (false);
		GameObjectMonster.gameObject.SetActive (false);

		int highestStage = PlayerPrefs.GetInt (PreferenceKeys.KEY_HIGHEST_STAGE, 1);
		int nextStage = PlayerPrefs.GetInt (PreferenceKeys.KEY_CURRENT_STAGE, 1) + 1;
		if (nextStage > highestStage) {
			PlayerPrefs.SetInt (PreferenceKeys.KEY_HIGHEST_STAGE, nextStage);
		}

		StartCoroutine (StartAnimationWin ());
	}

	public void OnLose() {
		GameObjectPlayer.gameObject.SetActive (false);
		GameObjectMonster.gameObject.SetActive (false);

		PlayerPrefs.SetInt (PreferenceKeys.KEY_CURRENT_STAGE, 1);

		StartCoroutine (StartAnimationLose ());
	}

	IEnumerator StartAnimationWin() {
		TransformResultPanel.gameObject.SetActive (true);
		SpriteLose.gameObject.SetActive (false);
		SpriteWin.GetComponent<Animator> ().Play ("Idle");
		yield return new WaitForSeconds (ResultPanelTransitionTime + 2);
		Application.LoadLevel (1);
		yield break;
	}

	IEnumerator StartAnimationLose() {
		TransformResultPanel.gameObject.SetActive (true);
		SpriteWin.gameObject.SetActive (false);
		SpriteLose.GetComponent<Animator> ().Play ("Idle");
		yield return new WaitForSeconds (ResultPanelTransitionTime + 2);
		Application.LoadLevel (3);
		yield break;
	}

}
