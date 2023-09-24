using System;
using System.Collections;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH129_Controller : CharacterControlBase {
    
	private void OnEnable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.AddUpdate(new ILogicUpdate(this.Pointer));
	}

	private void OnDisable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.RemoveUpdate(new ILogicUpdate(this.Pointer));
	}
	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
		this._refEntity.teleportInVoicePlayed = true;
	}

	private void InitializeSkill() {
		Transform transform = new GameObject("CustomShootPoint0").transform;
		transform.SetParent(base.transform);
		transform.localPosition = new Vector3(0f, 0.8f, 0f);
		Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		this._refEntity.ExtraTransforms = new Transform[2];
		this._refEntity.ExtraTransforms[0] = transform;
		this._refEntity.ExtraTransforms[1] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "R WeaponPoint", true);
		this._refEntity.PlayerSkills[0].ShootTransform[0] = this._refEntity.ExtraTransforms[0];
		this._refEntity.PlayerSkills[1].ShootTransform[0] = this._refEntity.ExtraTransforms[1];
		if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(this.sklFrenzyId, out this.sklFrenzy)) {
			this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this.sklFrenzy);
			MonoBehaviourSingleton<PoolManager>.Instance.CreatePoolBase<VirustrapBullet>("prefab/bullet/" + this.sklFrenzy.s_MODEL, this.sklFrenzy.s_MODEL, 3, null);
			this.frenzyBulletStatus.CopyWeaponStatus(this._refEntity.PlayerWeapons[1].weaponStatus, 0, 0);
		}
		this._regularEffect = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_ShagaruX_000", false).GetComponent<ParticleSystem>();
		this._regularEffect.Play(true);
		this._FrenzyEffect = new ParticleSystem[2];
		this._FrenzyEffect[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_crazydragon_000_L", false).GetComponent<ParticleSystem>();
		this._FrenzyEffect[1] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_crazydragon_000_R", false).GetComponent<ParticleSystem>();
		this._FrenzyEffect[0].Stop(true);
		this._FrenzyEffect[1].Stop(true);
		SKILL_TABLE skill_TABLE;
		if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(this._refEntity.CharacterData.n_PASSIVE_2, out skill_TABLE)) {
			int n_MOTION_DEF = skill_TABLE.n_MOTION_DEF;
			for (int i = 0; i < this._refEntity.tRefPassiveskill.listUsePassiveskill.Count; i++) {
				if (this._refEntity.tRefPassiveskill.listUsePassiveskill[i].tSKILL_TABLE.n_ID == n_MOTION_DEF) {
					this.canUseFrenzyBullet = true;
					break;
				}
			}
		}
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_0_00, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_00, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_2_00, 2, null);
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.TeleportOutCharacterDependEvt = (Callback)new Action(this.TeleportOutCharacterDepend);
		this._refEntity.StageTeleportOutCharacterDependEvt = (Callback)new Action(this.StageTeleportOutCharacterDepend);
		this._refEntity.StageTeleportInCharacterDependEvt = (Callback)new Action(this.StageTeleportInCharacterDepend);
	}

	public void LogicUpdate() {
		this.CheckFrenzyBuff();
		this.CheckFrenzyBullet();
		PerBuffManager refPBM = this._refEntity.selfBuffManager.sBuffStatus.refPBM;
		if (!this.spIsFull) {
			if (refPBM.nMeasureNow == refPBM.nMeasureMax) {
				this.spIsFull = true;
				base.PlaySkillSE("xa_ark01");
				return;
			}
		}
		else if (refPBM.nMeasureNow != refPBM.nMeasureMax) {
			this.spIsFull = false;
		}
	}

	private void CheckFrenzyBuff() {
		if (this._refEntity.PlayerSkills.Length != 0) {
			if (this._refEntity.PlayerSkills[0].Reload_index == 1) {
				if (!this.isFrenzyStatus) {
					this.PlayFrenzyFx();
					return;
				}
			}
			else if (this.isFrenzyStatus) {
				this.StopLightningFx();
			}
		}
	}

	private void CheckFrenzyBullet() {
		if (this._refEntity.IsLocalPlayer && this.isFrenzyStatus && GameLogicUpdateManager.GameFrame >= this.frenzySkillEventFrame) {
			if (MonoBehaviourSingleton<StageSyncManager>.Instance.bPauseAllPlayerInput) {
				return;
			}
			this.frenzySkillEventFrame = GameLogicUpdateManager.GameFrame + this.FRENZY_TIMER;
			if (this.canUseFrenzyBullet && this.sklFrenzy != null) {
				this._refEntity.PushBulletDetail(this.sklFrenzy, this.frenzyBulletStatus, this._refEntity.ModelTransform, 0);
			}
		}
	}

	private void PlayFrenzyFx() {
		this.isFrenzyStatus = true;
		this._FrenzyEffect[0].Play(true);
		this._FrenzyEffect[1].Play(true);
		FxManager_.Play(this.FX_2_00, this._refEntity.ModelTransform, OrangeCharacter.NormalQuaternion);
		base.PlaySkillSE("xa_ark02_lp");
	}

	private void StopLightningFx() {
		this.isFrenzyStatus = false;
		this._FrenzyEffect[0].Stop(true);
		this._FrenzyEffect[1].Stop(true);
		base.PlaySkillSE("xa_ark02_stop");
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 0) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			int num = (int)(this.risingTime / GameLogicUpdateManager.m_fFrameLen);
			ManagedSingleton<CharacterControlHelper>.Instance.TurnToAimTarget(this._refEntity);
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, num, num, OrangeCharacter.SubStatus.SKILL0, out this.skillEventFrame, out this.endFrame);
			this._refEntity.SetAnimateId(HumanBase.AnimateId.ANI_SKILL_START);
			this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			this._refEntity.IgnoreGravity = true;
			// this._refEntity.SetSpeed((int)(this._refEntity._characterDirection * (CharacterDirection)this.risingSpdX), this.risingSpdY);
			this._refEntity.SetSpeed(((int)this._refEntity._characterDirection * this.risingSpdX), this.risingSpdY);
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
			this._refEntity.CheckUsePassiveSkill(0, weaponStruct.weaponStatus, this._refEntity.ModelTransform);
			OrangeBattleUtility.UpdateSkillCD(weaponStruct);
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 1) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this.isSkillEventEnd = false;
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
			this._refEntity.IsShoot = 1;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_TRIGGER, this.SKL1_END, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)71U, (HumanBase.AnimateId)72U, (HumanBase.AnimateId)73U, true);
			this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
			this._refEntity.CheckUsePassiveSkill(1, weaponStruct.weaponStatus, weaponStruct.ShootTransform[0]);
			OrangeBattleUtility.UpdateSkillCD(weaponStruct);
		}
	}

	public override void CheckSkill() {
		if (this._refEntity.CurMainStatus != OrangeCharacter.MainStatus.SKILL) {
			return;
		}
		if (this._refEntity.IsAnimateIDChanged() || this._refEntity.CurrentActiveSkill == -1) {
			return;
		}
		this.nowFrame = GameLogicUpdateManager.GameFrame;
		OrangeCharacter.MainStatus curMainStatus = this._refEntity.CurMainStatus;
		if (curMainStatus == OrangeCharacter.MainStatus.SKILL) {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			switch (curSubStatus) {
			case OrangeCharacter.SubStatus.SKILL0:
				if (this.nowFrame >= this.endFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_STEP_2_START, this.SKL0_STEP_2_START, OrangeCharacter.SubStatus.SKILL0_1, out this.skillEventFrame, out this.endFrame);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)66U);
					this._refEntity.Dashing = false;
					this._refEntity.SetSpeed(0, 0);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_1:
				if (this.nowFrame >= this.endFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_STEP_2_LOOP, this.SKL0_STEP_2_LOOP, OrangeCharacter.SubStatus.SKILL0_2, out this.skillEventFrame, out this.endFrame);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)67U);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_2:
				if (this.nowFrame >= this.endFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_STEP_3_START, this.SKL0_STEP_3_START, OrangeCharacter.SubStatus.SKILL0_3, out this.skillEventFrame, out this.endFrame);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)68U);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_3:
				if (this.nowFrame >= this.endFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_STEP_3_LOOP, this.SKL0_STEP_3_LOOP, OrangeCharacter.SubStatus.SKILL0_4, out this.skillEventFrame, out this.endFrame);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)69U);
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ModelTransform, MagazineType.NORMAL, this._refEntity.GetCurrentSkillObj().Reload_index, 0, false);
					FxManager_.Play(this.FX_0_00, this._refEntity.AimTransform.position, OrangeCharacter.NormalQuaternion);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_4:
				if (this.nowFrame >= this.endFrame) {
					this.endBreakFrame = this.nowFrame + this.SKL0_STEP_3_BREAK;
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_STEP_3_END, this.SKL0_STEP_3_END, OrangeCharacter.SubStatus.SKILL0_5, out this.skillEventFrame, out this.endFrame);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)70U);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_5:
				if (this.nowFrame >= this.endFrame) {
					this.OnSkillEnd();
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
					return;
				}
				break;
			default:
				if (curSubStatus != OrangeCharacter.SubStatus.SKILL1) {
					return;
				}
				if (this.nowFrame >= this.endFrame) {
					this.OnSkillEnd();
					return;
				}
				if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
					this.isSkillEventEnd = true;
					ManagedSingleton<CharacterControlHelper>.Instance.UpdateShootDirByAimDir(this._refEntity);
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.PlayerSkills[1].ShootTransform[0], MagazineType.NORMAL, this._refEntity.GetCurrentSkillObj().Reload_index, 1, false);
					FxManager_.Play(this.FX_1_00, this._refEntity.PlayerSkills[1].ShootTransform[0].position, (this._refEntity.direction == 1) ? OrangeCharacter.NormalQuaternion : OrangeCharacter.ReversedQuaternion);
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
					ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
				}
				break;
			}
		}
	}

	private void OnSkillEnd() {
		if (this._refEntity.IgnoreGravity) {
			this._refEntity.IgnoreGravity = false;
		}
		this.isSkillEventEnd = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.EnableCurrentWeapon();
		HumanBase.AnimateId animateID = this._refEntity.AnimateID;
		if (animateID != (HumanBase.AnimateId)71U) {
			if (this._refEntity.Controller.Collisions.below || this._refEntity.Controller.Collisions.JSB_below) {
				this._refEntity.Dashing = false;
				this._refEntity.SetSpeed(0, 0);
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
				return;
			}
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
			return;
		}
		else {
			this._refEntity.Dashing = false;
			if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.DOWN)) {
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.CROUCH, OrangeCharacter.SubStatus.WIN_POSE);
				return;
			}
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.CROUCH_UP);
			return;
		}
	}

	public override void ClearSkill() {
		this.isSkillEventEnd = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.EnableCurrentWeapon();
	}

	public override void SetStun(bool enable) {
		base.SetStun(enable);
		this._refEntity.EnableCurrentWeapon();
	}

	public override void ControlCharacterDead() {
		this.ToggleRegularEffect(false);
	}

	public override void ControlCharacterContinue() {
		base.StartCoroutine((Il2CppSystem.Collections.IEnumerator)this.OnToggleRegularEffect(true, 0.6f));
	}

	private void TeleportOutCharacterDepend() {
		if (this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.TELEPORT_POSE) {
			float currentFrame = this._refEntity.CurrentFrame;
			if (currentFrame > 1.5f && currentFrame <= 2f) {
				this.ToggleRegularEffect(false);
			}
		}
	}

	protected void StageTeleportInCharacterDepend() {
		this.ToggleRegularEffect(false);
		base.StartCoroutine((Il2CppSystem.Collections.IEnumerator)this.OnToggleRegularEffect(true, 0.6f));
	}

	protected void StageTeleportOutCharacterDepend() {
		if (this._refEntity.CurMainStatus != OrangeCharacter.MainStatus.TELEPORT_OUT) {
			base.StartCoroutine((Il2CppSystem.Collections.IEnumerator)this.OnToggleRegularEffect(false, 0.2f));
			return;
		}
		if (!this._refEntity.Animator.IsDefaultAnimator) {
			base.StartCoroutine((Il2CppSystem.Collections.IEnumerator)this.OnToggleRegularEffect(false, 0.2f));
		}
	}

	private IEnumerator OnToggleRegularEffect(bool isActive, float delay) {
		yield return new WaitForSeconds(delay);
		this.ToggleRegularEffect(isActive);
		yield break;
	}

	private void ToggleRegularEffect(bool isActive) {
		if (isActive) {
			this._regularEffect.Play(true);
			return;
		}
		this._regularEffect.Stop(true);
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch129_skill_01_step1_loop",
				"ch129_skill_01_step2_start",
				"ch129_skill_01_step2_loop",
				"ch129_skill_01_step3_start",
				"ch129_skill_01_step3_loop",
				"ch129_skill_01_step3_end",
				"ch129_skill_02_crouch",
				"ch129_skill_02_stand",
				"ch129_skill_02_jump"
			}
		);
	}

	private readonly int sklFrenzyId = 22570;

	private int nowFrame;

	private int skillEventFrame;

	private int endFrame;

	private int endBreakFrame;

	private bool isSkillEventEnd;

	private bool isFrenzyStatus;

	private int frenzySkillEventFrame;

	private SKILL_TABLE sklFrenzy;

	// [SerializeField]
	private int risingSpdX = 2000;

	// [SerializeField]
	private int risingSpdY = 15000;

	// [SerializeField]
	private float risingTime = 0.1f;

	private ParticleSystem _regularEffect;

	private ParticleSystem[] _FrenzyEffect;

	private WeaponStatus frenzyBulletStatus = new WeaponStatus();

	private bool canUseFrenzyBullet;

	private bool spIsFull = true;

	private readonly string FX_0_00 = "fxuse_ShagaruX_000";

	private readonly string FX_1_00 = "fxuse_blackpoisonball_000";

	private readonly string FX_2_00 = "fxhit_barrage_002";

	protected readonly int SKL0_STEP_1_LOOP = (int)(0.2f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_2_START = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_2_LOOP = (int)(0.2f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_3_START = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_3_LOOP = (int)(1.5f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_3_END = (int)(0.267f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_STEP_3_BREAK = (int)(0.187f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_TRIGGER = (int)(0.192f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_END = (int)(0.833f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_END_BREAK = (int)(0.4f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int FRENZY_TIMER = (int)(2f / GameLogicUpdateManager.m_fFrameLen);
}
