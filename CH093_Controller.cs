using System;
using System.Collections.Generic;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace OrangeMods;

public class CH093_Controller : CharacterControllerProxyBaseGen3 {
	private void InitializeLinkSkillData() {
		int n_LINK_SKILL = this._refEntity.PlayerSkills[0].BulletData.n_LINK_SKILL;
		if (n_LINK_SKILL == 0) {
			return;
		}
		SKILL_TABLE linkSkillData;
		if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(n_LINK_SKILL, out linkSkillData)) {
			this._refEntity.tRefPassiveskill.ReCalcuSkill(ref linkSkillData);
			this._linkSkillData = linkSkillData;
		}
	}

	private void ToggleMeleeBullet(bool isActive, bool useLinkSkill = false) {
		if (isActive) {
			if (!this._isMeleeBulletSetted) {
				WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
				if (useLinkSkill && this._linkSkillData != null) {
					BulletBase bulletCollider = this._refEntity.BulletCollider;
					SKILL_TABLE linkSkillData = this._linkSkillData;
					string sPlayerName = this._refEntity.sPlayerName;
					int nowRecordNO = this._refEntity.GetNowRecordNO();
					OrangeCharacter refEntity = this._refEntity;
					int nBulletRecordID = refEntity.nBulletRecordID;
					refEntity.nBulletRecordID = nBulletRecordID + 1;
					bulletCollider.UpdateBulletData(linkSkillData, sPlayerName, nowRecordNO, nBulletRecordID, this._refEntity.direction);
				}
				else {
					BulletBase bulletCollider2 = this._refEntity.BulletCollider;
					SKILL_TABLE bulletData = currentSkillObj.BulletData;
					string sPlayerName2 = this._refEntity.sPlayerName;
					int nowRecordNO2 = this._refEntity.GetNowRecordNO();
					OrangeCharacter refEntity2 = this._refEntity;
					int nBulletRecordID = refEntity2.nBulletRecordID;
					refEntity2.nBulletRecordID = nBulletRecordID + 1;
					bulletCollider2.UpdateBulletData(bulletData, sPlayerName2, nowRecordNO2, nBulletRecordID, this._refEntity.direction);
				}
				this._refEntity.BulletCollider.SetBulletAtk(currentSkillObj.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
				this._refEntity.BulletCollider.BulletLevel = currentSkillObj.SkillLV;
				this._refEntity.BulletCollider.Active(this._refEntity.TargetMask);
				this._isMeleeBulletSetted = true;
				return;
			}
		}
		else {
			this._refEntity.BulletCollider.BackToPool();
			this._isMeleeBulletSetted = false;
		}
	}

	private void OnBulletColliderHit(Il2CppSystem.Object obj) {
		base.StartHitPause(this.TIME_SKILL_0_HITPAUSE);
	}

	private void ActionStatusChanged_0_0() {
		base.SetIgnoreGravity(true);
		WeaponStruct weaponStruct = this._refEntity.PlayerSkills[base.CurActiveSkill];
		this._refEntity.CheckUsePassiveSkill(base.CurActiveSkill, weaponStruct.weaponStatus, this._refEntity.ExtraTransforms[0]);
		OrangeBattleUtility.UpdateSkillCD(weaponStruct);
		this._refEntity.SetAnimateId(HumanBase.AnimateId.ANI_SKILL_START);
		base.AnimatorSpeed = this.SkillSpeed;
		base.SetSkillFrame(this.Time_Skill_0_Raise / this.SkillSpeed);
	}

	private void ActionStatusChanged_0_1() {
		base.SetSpeed((float)this.SPEED_SKILL_0_BASE * this.Speed_0_Ratio_Chop_1 * this.SkillSpeed * (float)this._refEntity.direction, 0f);
		base.SetSkillFrame(this.Time_Skill_0_ChopShift_1 / this.SkillSpeed);
	}

	private void ActionStatusChanged_0_2() {
		this.ToggleMeleeBullet(false, false);
		base.SetSpeed((float)this.SPEED_SKILL_0_BASE * this.Speed_0_Ratio_Roll * (float)this._refEntity.direction, 0f);
		base.SetSkillFrame(this.Time_Skill_0_Roll / this.SkillSpeed);
	}

	private void ActionStatusChanged_0_3() {
		base.SetSpeed((float)this.SPEED_SKILL_0_BASE * this.Speed_0_Ratio_Chop_2 * this.SkillSpeed * (float)this._refEntity.direction, 0f);
		base.SetSkillFrame(this.Time_Skill_0_ChopShift_2 / this.SkillSpeed);
	}

	private void ActionStatusChanged_0_4() {
		this.ToggleMeleeBullet(false, false);
		base.ResetSpeed();
		base.SetSkillFrame(this.Time_Skill_0_Chop_Cancel / this.SkillSpeed);
	}

	private void ActionStatusChanged_1_0() {
		base.SetIgnoreGravity(true);
		this._refEntity.SetAnimateId((HumanBase.AnimateId)66U);
		base.SetSkillFrame(this.Time_Skill_1_Raise / this.SkillSpeed);
		Plugin.FxManagerPlay(CH093_Controller.FxName.fxuse_dragonslash_000.ToString(), this._refEntity._transform, OrangeCharacter.NormalQuaternion, new Vector3((float)base.SkillFXDirection, 1f, 1f));
	}

	private void ActionStatusChanged_1_1() {
		base.SetSpeed((float)this.SPEED_SKILL_1_BASE * this.Speed_1_Ratio_Slash * (float)this._refEntity.direction, 0f);
		base.SetSkillFrame(this.Time_Skill_1_Slash);
	}

	private void ActionStatusChanged_1_2() {
		base.ResetSpeed();
		WeaponStruct weaponStruct = this._refEntity.PlayerSkills[base.CurActiveSkill];
		SKILL_TABLE skill_TABLE = weaponStruct.FastBulletDatas[weaponStruct.Reload_index];
		this._refEntity.CheckUsePassiveSkill(base.CurActiveSkill, weaponStruct.weaponStatus, this._refEntity.ExtraTransforms[0]);
		this._refEntity.RemoveComboSkillBuff(skill_TABLE.n_ID);
		OrangeBattleUtility.UpdateSkillCD(weaponStruct);
		this._refEntity.IsShoot = 0;
		this._refEntity.StartShootTimer();
		this._refEntity.PushBulletDetail(skill_TABLE, weaponStruct.weaponStatus, this._refEntity.ExtraTransforms[0], weaponStruct.SkillLV, new Il2CppSystem.Nullable_Unboxed<UnityEngine.Vector3>(Vector2.right * (float)this._refEntity.direction), true);
		base.SetSkillCancelFrame(this.Time_Skill_1_Slash_Cancel);
	}

	private void ActionLogicUpdate_0() {
		if (base.IsHitPauseStarted) {
			base.AddSkillEndFrame(1);
		}
		this.ToggleMeleeBullet(true, base.CurSubStatus == OrangeCharacter.SubStatus.SKILL0_3);
		if (base.CheckSkillFrameEnd()) {
			Plugin.FxManagerPlay((base.CurSubStatus == OrangeCharacter.SubStatus.SKILL0_3) ? CH093_Controller.FxName.fxuse_chargecut_002.ToString() : CH093_Controller.FxName.fxuse_chargecut_001.ToString(), this._refEntity._transform.position + Vector3.right * this.Shift_Skill_0_FX * (float)this._refEntity.direction, (this._refEntity.ShootDirection.x > 0f) ? OrangeCharacter.NormalQuaternion : OrangeCharacter.ReversedQuaternion);
			base.ShiftSkillStatus();
		}
	}

	public override void Awake() {
		base.Awake();
		if (this._refEntity is OrangeConsoleCharacter) {
			this._refConsolePlayer = (this._refEntity as OrangeConsoleCharacter);
		}
		this._refEntity.ExtraTransforms = new Transform[] {
			OrangeBattleUtility.FindChildRecursive(base.ChildTransforms, "L WeaponPoint", true),
			OrangeBattleUtility.FindChildRecursive(base.ChildTransforms, "R WeaponPoint", true)
		};
		this._busterMesh = OrangeBattleUtility.FindChildRecursive(base.ChildTransforms, "BusterMesh_m", false).GetComponent<SkinnedMeshRenderer>();
		this._saberMesh = OrangeBattleUtility.FindChildRecursive(base.ChildTransforms, "SaberMesh_m", true).GetComponent<SkinnedMeshRenderer>();
		this._backSaberMesh = OrangeBattleUtility.FindChildRecursive(base.ChildTransforms, "BackSaberMesh_m", false).GetComponent<SkinnedMeshRenderer>();
	}

	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		foreach (string fxName in Enum.GetNames(typeof(CH093_Controller.FxName))) {
			MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(fxName, 2, null);
		}
		CollideBullet bulletCollider = this._refEntity.BulletCollider;
		bulletCollider.HitCallback = (CallbackObj)Il2CppSystem.Delegate.Combine(bulletCollider.HitCallback, (CallbackObj)new Action<Il2CppSystem.Object>(this.OnBulletColliderHit));
		this.InitializeLinkSkillData();

		// TODO
		/*
		base.InitializeSkillDependDelegators(
			new Il2CppSystem.Collections.Generic.Dictionary<OrangeCharacter.SubStatus, CharacterControllerProxyBaseGen3.SkillStateDelegateData> ((
				new Dictionary<OrangeCharacter.SubStatus, CharacterControllerProxyBaseGen3.SkillStateDelegateData> { {
						OrangeCharacter.SubStatus.SKILL0,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_0_0),
							OnLogicUpdate = new Action(base.ActionCheckNextSkillStatus)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL0_1,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_0_1),
							OnLogicUpdate = new Action(this.ActionLogicUpdate_0)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL0_2,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_0_2),
							OnLogicUpdate = new Action(base.ActionCheckNextSkillStatus)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL0_3,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_0_3),
							OnLogicUpdate = new Action(this.ActionLogicUpdate_0)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL0_4,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_0_4),
							OnAnimationEnd = new Action(base.ActionSetSkillEnd),
							OnLogicUpdate = new Action(base.ActionCheckSkillCancel)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL1,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_1_0),
							OnLogicUpdate = new Action(base.ActionCheckNextSkillStatus)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL1_1,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnStatusChanged = new Action(this.ActionStatusChanged_1_1),
							OnLogicUpdate = new Action(base.ActionCheckNextSkillStatus)
						}
					}, {
						OrangeCharacter.SubStatus.SKILL1_2,
						new CharacterControllerProxyBaseGen3.SkillStateDelegateData {
							OnAnimationEnd = new Action(base.ActionSetSkillEnd),
							OnStatusChanged = new Action(this.ActionStatusChanged_1_2),
							OnLogicUpdate = new Action(base.ActionCheckSkillCancel)
						}
					}
				}
			))
		);
		*/
	}

	public override void OnDestroy() {
		base.OnDestroy();
		CollideBullet bulletCollider = this._refEntity.BulletCollider;
		bulletCollider.HitCallback = (CallbackObj)Il2CppSystem.Delegate.Remove(bulletCollider.HitCallback, (CallbackObj)new Action<Il2CppSystem.Object>(this.OnBulletColliderHit));
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch093_skill_01",
				"ch093_skill_02_step2"
			}
		);
	}

	public override void GetUniqueMotion(out Il2CppStringArray source, out Il2CppStringArray target) {
		source = new Il2CppStringArray(
			new string[] {
				"login",
				"logout",
				"win",
				"buster_stand_charge_atk",
				"buster_crouch_charge_atk",
				"buster_jump_charge_atk",
				"buster_fall_charge_atk",
				"buster_wallgrab_charge_atk"
			}
		);
		target = new Il2CppStringArray(
			new string[] {
				"ch093_login",
				"ch093_logout",
				"ch093_win",
				"ch093_skill_02_step1_stand",
				"ch093_skill_02_step1_Crouch",
				"ch093_skill_02_step1_Jump",
				"ch093_skill_02_step1_Fall",
				"ch093_skill_02_step1_Wallgrab"
			}
		);
	}

	public override void TeleportInCharacterDepend() {
	}

	public override void TeleportOutCharacterDepend() {
		if (this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.TELEPORT_POSE) {
			float currentFrame = this._refEntity.CurrentFrame;
			if (currentFrame > 1.5f && currentFrame <= 2f) {
				this._busterMesh.enabled = false;
				this._backSaberMesh.enabled = false;
				this._saberMesh.enabled = false;
			}
		}
	}

	public override void StageTeleportInCharacterDepend() {
		base.StartCoroutine(base.ToggleExtraTransforms(true, 0.5f));
	}

	public override void StageTeleportOutCharacterDepend() {
		base.StartCoroutine(base.ToggleExtraTransforms(false, 0.3f));
	}

	public override void ControlCharacterDead() {
		base.ForceStopHitPause();
		base.StartCoroutine(base.ToggleExtraTransforms(false, 0.5f));
	}

	public override void ControlCharacterContinue() {
		base.StartCoroutine(base.ToggleExtraTransforms(true, 0.6f));
	}

	public override void ToggleExtraTransforms(bool isActive) {
		this._backSaberMesh.enabled = isActive;
	}

	public override void SetStun(bool enable) {
		if (enable) {
			base.ForceStopHitPause();
		}
	}

	public override void ToggleWeapon(CharacterControllerProxyBaseGen1.WeaponState weaponState) {
		switch (weaponState) {
		case CharacterControllerProxyBaseGen1.WeaponState.TELEPORT_IN:
		case CharacterControllerProxyBaseGen1.WeaponState.TELEPORT_OUT:
			base.ToggleNormalWeapon(false);
			this._busterMesh.enabled = true;
			this._backSaberMesh.enabled = true;
			this._saberMesh.enabled = false;
			this._refEntity.EnableHandMesh(false);
			return;
		case CharacterControllerProxyBaseGen1.WeaponState.SKILL_0:
			base.ToggleNormalWeapon(false);
			this._busterMesh.enabled = false;
			this._backSaberMesh.enabled = false;
			this._saberMesh.enabled = true;
			return;
		case CharacterControllerProxyBaseGen1.WeaponState.SKILL_1: {
			base.ToggleNormalWeapon(false);
			int reload_index = this._refEntity.PlayerSkills[this._refEntity.CurrentActiveSkill].Reload_index;
			if (reload_index == 0) {
				this._busterMesh.enabled = true;
				this._backSaberMesh.enabled = true;
				this._saberMesh.enabled = false;
				this._refEntity.EnableHandMesh(false);
				return;
			}
			if (reload_index != 1) {
				return;
			}
			this._busterMesh.enabled = false;
			this._backSaberMesh.enabled = false;
			this._saberMesh.enabled = true;
			return;
		}
		}
		base.ToggleNormalWeapon(true);
		this._busterMesh.enabled = false;
		this._backSaberMesh.enabled = true;
		this._saberMesh.enabled = false;
	}

	public override void SetSkillEnd() {
		this._isSkillShooting = false;
		base.SetSkillEnd();
	}

	public override void OnChangeComboSkill(CharacterControllerProxyBaseGen1.SkillID skillId, int reloadIndex) {
		if (skillId == CharacterControllerProxyBaseGen1.SkillID.SKILL_1) {
			if (reloadIndex != 0) {
				if (reloadIndex != 1) {
					return;
				}
				OrangeConsoleCharacter refConsolePlayer = this._refConsolePlayer;
				if (refConsolePlayer != null) {
					refConsolePlayer.SetVirtualButtonAnalog(VirtualButtonId.SKILL1, false);
				}
				OrangeConsoleCharacter refConsolePlayer2 = this._refConsolePlayer;
				if (refConsolePlayer2 == null) {
					return;
				}
				refConsolePlayer2.ClearVirtualButtonStick(VirtualButtonId.SKILL1);
			}
			else {
				OrangeConsoleCharacter refConsolePlayer3 = this._refConsolePlayer;
				if (refConsolePlayer3 == null) {
					return;
				}
				refConsolePlayer3.SetVirtualButtonAnalog(VirtualButtonId.SKILL1, true);
				return;
			}
		}
	}

	public override void AttachSkillDelegateEvent() {
		this.OnPlayerPressSkill0Events[0] = new Action<CharacterControllerProxyBaseGen1.SkillID>(this.OnPlayerPressSkill0);
		this.OnPlayerPressSkill1Events[1] = new Action<CharacterControllerProxyBaseGen1.SkillID>(this.OnPlayerPressSkill1_1);
		this.OnPlayerReleaseSkill1Events[0] = new Action<CharacterControllerProxyBaseGen1.SkillID>(this.OnPlayerReleaseSkill1_0);
	}

	public override void OnPlayerPressSkill0(CharacterControllerProxyBaseGen1.SkillID skillID) {
		base.OnPlayerPressSkill0(skillID);
		base.PlayVoiceSE("v_xm_skill01");
		base.PlaySkillSE("xm_tame01");
		Plugin.FxManagerPlay(CH093_Controller.FxName.fxuse_chargecut_000.ToString(), this._refEntity._transform.position, (this._refEntity.ShootDirection.x > 0f) ? OrangeCharacter.NormalQuaternion : OrangeCharacter.ReversedQuaternion);
	}

	protected void OnPlayerPressSkill1_1(CharacterControllerProxyBaseGen1.SkillID skillID) {
		base.PlayVoiceSE("v_xm_skill02_2");
		base.OnPlayerPressSkill1(skillID);
	}

	private void OnPlayerReleaseSkill1_0(CharacterControllerProxyBaseGen1.SkillID skillID) {
		WeaponStruct weaponStruct = this._refEntity.PlayerSkills[(int)skillID];
		base.SetSkillAndWeapon(skillID);
		this._refEntity.IsShoot = 3;
		this._refEntity.StartShootTimer();
		this._refEntity.Animator.SetAnimatorEquip(1);
		this._refEntity.PushBulletDetail(weaponStruct.BulletData, weaponStruct.weaponStatus, this._refEntity.ExtraTransforms[0], weaponStruct.SkillLV, new Il2CppSystem.Nullable_Unboxed<UnityEngine.Vector3>(this._refEntity.ShootDirection), true);
		this._isSkillShooting = true;
		this._refEntity.CheckUsePassiveSkill((int)skillID, weaponStruct.weaponStatus, this._refEntity.ExtraTransforms[0]);
		base.PlayVoiceSE("v_xm_skill02_1");
	}

	public override void LogicUpdateCharacterDepend() {
		CharacterControllerProxyBaseGen1.SkillID curActiveSkill = (CharacterControllerProxyBaseGen1.SkillID)base.CurActiveSkill;
		if (curActiveSkill == CharacterControllerProxyBaseGen1.SkillID.SKILL_1 && this._isSkillShooting && this._refEntity.CheckSkillEndByShootTimer()) {
			this._isSkillShooting = false;
			this.ToggleWeapon(CharacterControllerProxyBaseGen1.WeaponState.NORMAL);
		}
		base.LogicUpdateCharacterDepend();
	}

	public override int GetUniqueWeaponType() {
		return 1;
	}

	public override void ClearSkill() {
		CharacterControllerProxyBaseGen1.SkillID currentActiveSkill = (CharacterControllerProxyBaseGen1.SkillID)this._refEntity.CurrentActiveSkill;
		if (currentActiveSkill == CharacterControllerProxyBaseGen1.SkillID.SKILL_0) {
			this.SetSkillEnd();
			return;
		}
		if (currentActiveSkill != CharacterControllerProxyBaseGen1.SkillID.SKILL_1) {
			return;
		}
		if (this._isSkillShooting) {
			this._refEntity.CancelBusterChargeAtk();
			this._isSkillShooting = false;
			this.ToggleWeapon(CharacterControllerProxyBaseGen1.WeaponState.NORMAL);
		}
		this.SetSkillEnd();
	}

	public readonly float TIME_SKILL_0_HITPAUSE = 0.3f;

	private readonly int SPEED_SKILL_0_BASE = Mathf.RoundToInt(OrangeBattleUtility.PlayerWalkSpeed * OrangeBattleUtility.PPU * OrangeBattleUtility.FPS * 1000f);

	private readonly int SPEED_SKILL_1_BASE = Mathf.RoundToInt(OrangeBattleUtility.PlayerWalkSpeed * OrangeBattleUtility.PPU * OrangeBattleUtility.FPS * 1000f);

	public float Shift_Skill_0_FX = 1.5f;

	public float Speed_0_Ratio_Chop_1 = 1f;

	public float Speed_0_Ratio_Roll = 1.5f;

	public float Speed_0_Ratio_Chop_2 = 2f;

	public float Time_Skill_0_Raise = 0.4f;

	public float Time_Skill_0_ChopShift_1 = 0.1f;

	public float Time_Skill_0_Roll = 0.3f;

	public float Time_Skill_0_ChopShift_2 = 0.1f;

	public float Time_Skill_0_Chop_Cancel = 0.2f;

	public float Speed_1_Ratio_Slash = 0.5f;

	public float Time_Skill_1_Raise = 0.1f;

	public float Time_Skill_1_Slash = 0.3f;

	public float Time_Skill_1_Slash_Cancel = 0.15f;

	private bool _isSkillShooting;

	private SkinnedMeshRenderer _busterMesh;

	private SkinnedMeshRenderer _saberMesh;

	private SkinnedMeshRenderer _backSaberMesh;

	private bool _isMeleeBulletSetted;

	public float SkillSpeed = 1f;

	private SKILL_TABLE _linkSkillData;

	private OrangeConsoleCharacter _refConsolePlayer;

	private enum SkillAnimationId : uint {
			ANI_SKILL0 = 65U,
			ANI_SKILL1_SLASH
	}

	private enum FxName {
		fxuse_chargecut_000,
		fxuse_chargecut_001,
		fxuse_chargecut_002,
		fxuse_dragonslash_000
	}
}
