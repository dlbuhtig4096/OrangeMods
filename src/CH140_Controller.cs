
using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH140_Controller : CharacterControlBase {

	public CH140_Controller() : base() {}
	
	public CH140_Controller(IntPtr p) : base(p) {}
    
	private void OnEnable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.AddUpdate(new ILogicUpdate(this.Pointer));
	}

	private void OnDisable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.RemoveUpdate(new ILogicUpdate(this.Pointer));
	}

	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
		this._refPlayer = (this._refEntity as OrangeConsoleCharacter);
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
		this._refEntity.PlayerSkills[1].ShootTransform[0] = this._refEntity.ExtraTransforms[0];
		this._FrenzyEffect = new ParticleSystem[1];
		this._FrenzyEffect[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_DMCXDevilTrigger_000_(work)", false).GetComponent<ParticleSystem>();
		this._FrenzyEffect[0].gameObject.SetActive(false);
		this._teleportEffect = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxdemo_DMCX_003", false).GetComponent<ParticleSystem>();
		GameObject gameObject = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "SwordMesh_m", true).gameObject;
		if (gameObject) {
			this.cmSaber = gameObject.GetComponent<CharacterMaterial>();
		}
		GameObject gameObject2 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "BackSwordMesh_m", true).gameObject;
		if (gameObject2) {
			this.cmSaberBack = gameObject2.GetComponent<CharacterMaterial>();
		}
		GameObject gameObject3 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "GunMesh_L_m", true).gameObject;
		if (gameObject3) {
			this.cmGun = gameObject3.GetComponent<CharacterMaterial>();
		}
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_0_00, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_0_01, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_00, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_01, 2, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_2_00, 2, null);
		this.NOVASTRIKETimer = OrangeTimerManager.GetTimer(TimerMode.FRAME);
		this.isInit = true;
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
		this._refEntity.PlayTeleportOutEffectEvt = (Callback)new Action(this.PlayTeleportOutEffect);
		this._refEntity.TeleportOutCharacterDependEvt = (Callback)new Action(this.TeleportOutCharacterDepend);
		if (this._refEntity is OrangeConsoleCharacter) {
			this._refEntity.ChangeComboSkillEventEvt = (CallbackObjs)new Action<Il2CppReferenceArray<Il2CppSystem.Object>>(this.ChangeComboSkillEvent);
		}
	}

	private void PlayTeleportOutEffect() {
		Vector3 p_worldPos = base.transform.position;
		if (this._refEntity != null) {
			p_worldPos = this._refEntity.AimPosition;
		}
		FxManager_.Play("FX_TELEPORT_OUT", p_worldPos, Quaternion.identity);
	}

	public void LogicUpdate() {
		if (this.isInit) {
			this.CheckFrenzyBuff();
		}
	}

	private void CheckFrenzyBuff() {
		if (this._refEntity.PlayerSkills.Length != 0) {
			if (this._refEntity.PlayerSkills[0].Reload_index == 2) {
				if (!this.isFrenzyStatus) {
					this.PlayFrenzyFx();
					return;
				}
			}
			else if (this.isFrenzyStatus) {
				this.StopFrenzyFx();
			}
		}
	}

	private void PlayFrenzyFx() {
		this.isFrenzyStatus = true;
		this._FrenzyEffect[0].gameObject.SetActive(true);
		this._FrenzyEffect[0].Play(true);
		FxManager_.Play(this.FX_2_00, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
		base.PlaySkillSE("xc_majin01");
	}

	private void StopFrenzyFx() {
		this.isFrenzyStatus = false;
		this._FrenzyEffect[0].gameObject.SetActive(false);
		this._FrenzyEffect[0].Stop(true);
		base.PlaySkillSE("xc_majin02");
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id != 0) {
			if (id != 1) {
				return;
			}
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this.isSkillEventEnd = false;
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_TRIGGER, this.SKL1_LOOP, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
			this._refEntity.SetAnimateId((HumanBase.AnimateId)70U);
			this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			this.ToggleGun(true);
			this._refEntity.IgnoreGravity = true;
			this._refEntity.SetSpeed(0, this.risingSpdY);
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
			this._refEntity.CheckUsePassiveSkill(1, weaponStruct.weaponStatus, weaponStruct.ShootTransform[0]);
			OrangeBattleUtility.UpdateSkillCD(weaponStruct);
			FxManager_.Play(this.FX_1_00, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
			base.PlayVoiceSE("v_xc_skill02");
			base.PlaySkillSE("xc_rain01");
			return;
		}
		else {
			if (!this.IsUseStinger()) {
				return;
			}
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this.ToggleSaber(true, true);
			this.skillEventFrame = GameLogicUpdateManager.GameFrame + ManagedSingleton<CharacterControlHelper>.Instance.NOVASTRIKE_PREPARE_FRAME;
			ManagedSingleton<CharacterControlHelper>.Instance.NOVASTRIKE_Prepare(this._refEntity, 0);
			base.PlayVoiceSE("v_xc_skill01");
			base.PlaySkillSE("xc_stinger01");
			return;
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 0) {
			if (this.IsUseStinger()) {
				return;
			}
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this.isSkillEventEnd = false;
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
			this._refEntity.IsShoot = 1;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_1_TRIGGER, this.SKL0_1_TRIGGER, OrangeCharacter.SubStatus.SKILL0, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)68U, (HumanBase.AnimateId)68U, (HumanBase.AnimateId)69U, true);
			this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			this.ToggleSaber(true, true);
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
			this._refEntity.CheckUsePassiveSkill(0, weaponStruct.weaponStatus, weaponStruct.ShootTransform[0]);
			OrangeBattleUtility.UpdateSkillCD(weaponStruct);
			this._refEntity.RemoveComboSkillBuff(weaponStruct.FastBulletDatas[1].n_ID);
			FxManager_.Play(this.FX_0_01, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
			base.PlayVoiceSE("v_xc_skill01");
			base.PlaySkillSE("xc_stinger04");
		}
	}

	public override void CheckSkill() {
		if (this._refEntity.IsAnimateIDChanged()) {
			return;
		}
		if (this._refEntity.CurMainStatus != OrangeCharacter.MainStatus.SKILL) {
			return;
		}
		if (this._refEntity.CurrentActiveSkill == -1) {
			return;
		}
		this.nowFrame = GameLogicUpdateManager.GameFrame;
		OrangeCharacter.MainStatus curMainStatus = this._refEntity.CurMainStatus;
		if (curMainStatus == OrangeCharacter.MainStatus.SKILL) {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			if (curSubStatus <= OrangeCharacter.SubStatus.IDLE) {
				if (curSubStatus != OrangeCharacter.SubStatus.RIDE_ARMOR) {
					if (curSubStatus != OrangeCharacter.SubStatus.IDLE) {
						return;
					}
					if (ManagedSingleton<CharacterControlHelper>.Instance.NOVASTRIKE_Loop(this._refEntity, this.NOVASTRIKETimer, 0)) {
						this.ToggleSaber(false, true);
						return;
					}
				}
				else {
					if (this._refEntity.CurrentActiveSkill != 0) {
						this._refEntity.CurrentActiveSkill = 0;
					}
					if (this.nowFrame >= this.skillEventFrame) {
						ManagedSingleton<CharacterControlHelper>.Instance.NOVASTRIKE_Prepare_To_Loop(this._refEntity, this.NOVASTRIKETimer, 0, true, false);
						base.PlaySkillSE("xc_stinger02");
						this._refEntity.PlaySE(this._refEntity.SkillSEID, "xc_stinger03", 0.07f, false, true);
						return;
					}
				}
			}
			else {
				switch (curSubStatus) {
				case OrangeCharacter.SubStatus.SKILL0:
					if (this.nowFrame >= this.endFrame) {
						WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
						ManagedSingleton<CharacterControlHelper>.Instance.UpdateShootDirByAimDir(this._refEntity);
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct.ShootTransform[0], MagazineType.NORMAL, this.GetSkill0ReloadIdx(), 1, false);
						this.shootDirection = this._refEntity.ShootDirection;
						int num = this.SKL0_2_TRIGGER - this.SKL0_1_TRIGGER;
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, num, num, OrangeCharacter.SubStatus.SKILL0_1, out this.skillEventFrame, out this.endFrame);
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL0_1:
					if (this.nowFrame >= this.endFrame) {
						WeaponStruct weaponStruct2 = this._refEntity.PlayerSkills[0];
						ManagedSingleton<CharacterControlHelper>.Instance.UpdateShootDirByAimDir(this._refEntity);
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct2.ShootTransform[0], this.shootDirection, MagazineType.NORMAL, this.GetSkill0ReloadIdx(), 0, false);
						this.shootDirection = this._refEntity.ShootDirection;
						int p_sklTriggerFrame = this.SKL0_3_TRIGGER - this.SKL0_2_TRIGGER;
						int p_endFrame = this.SKL0_END - this.SKL0_2_TRIGGER;
						this.isSkillEventEnd = false;
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, p_sklTriggerFrame, p_endFrame, OrangeCharacter.SubStatus.SKILL0_2, out this.skillEventFrame, out this.endFrame);
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL0_2:
					if (this.nowFrame >= this.endFrame) {
						this.OnSkillEnd();
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						WeaponStruct weaponStruct3 = this._refEntity.PlayerSkills[0];
						ManagedSingleton<CharacterControlHelper>.Instance.UpdateShootDirByAimDir(this._refEntity);
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct3.ShootTransform[0], this.shootDirection, MagazineType.NORMAL, this.GetSkill0ReloadIdx(), 0, false);
						this.shootDirection = this._refEntity.ShootDirection;
						return;
					}
					if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
						ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
						return;
					}
					break;
				default:
					if (curSubStatus != OrangeCharacter.SubStatus.SKILL1) {
						if (curSubStatus != OrangeCharacter.SubStatus.SKILL1_1) {
							return;
						}
						if (this.nowFrame >= this.endFrame) {
							this.OnSkillEnd();
							return;
						}
						if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
							ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
						}
					}
					else {
						if (this.nowFrame >= this.endFrame) {
							this._refEntity.SetAnimateId((HumanBase.AnimateId)72U);
							ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL1_END, this.SKL1_END, OrangeCharacter.SubStatus.SKILL1_1, out this.skillEventFrame, out this.endFrame);
							return;
						}
						if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
							this.isSkillEventEnd = true;
							WeaponStruct weaponStruct4 = this._refEntity.PlayerSkills[1];
							ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct4.ShootTransform[0], MagazineType.NORMAL, this._refEntity.GetCurrentSkillObj().Reload_index, 0, false);
							this._refEntity.SetAnimateId((HumanBase.AnimateId)71U);
							this._refEntity.Dashing = false;
							this._refEntity.SetSpeed(0, 0);
							FxManager_.Play(this.FX_1_01, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
							base.PlaySkillSE("xc_rain02");
							return;
						}
					}
					break;
				}
			}
		}
	}

	public void SetStatusCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
		if (mainStatus != OrangeCharacter.MainStatus.TELEPORT_OUT) {
			if (mainStatus == OrangeCharacter.MainStatus.SKILL && subStatus == OrangeCharacter.SubStatus.IDLE) {
				FxManager_.Play(this.FX_0_00, this._refEntity.ModelTransform, Quaternion.identity);
				return;
			}
		}
		else {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			if (curSubStatus == OrangeCharacter.SubStatus.WIN_POSE) {
				this.isWinPose = true;
				return;
			}
			this.teleportFxFrame = GameLogicUpdateManager.GameFrame + this.LOGOUT_TRIGGER;
		}
	}

	private void TeleportOutCharacterDepend() {
		if (!this.isWinPose) {
			return;
		}
		if (this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.TELEPORT_POSE && GameLogicUpdateManager.GameFrame == this.teleportFxFrame && this._teleportEffect) {
			this._teleportEffect.Play(true);
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
		this.ToggleSaber(false, true);
		this.ToggleGun(false);
		HumanBase.AnimateId animateID = this._refEntity.AnimateID;
		if (this._refEntity.IsInGround) {
			this._refEntity.Dashing = false;
			this._refEntity.SetSpeed(0, 0);
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
			return;
		}
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
	}

	private bool IsUseStinger() {
		return !this._refEntity.BuffManager.CheckHasEffectByCONDITIONID(this.comboId, 0);
	}

	private int GetSkill0ReloadIdx() {
		if (this._refEntity.BuffManager.CheckHasEffectByCONDITIONID(this.conditionId, 0)) {
			return 2;
		}
		return 1;
	}

	private void ToggleSaber(bool enable, bool setBack = true) {
		if (this.cmSaber) {
			if (enable) {
				this.cmSaber.Appear(null, -1f);
				if (setBack) {
					this.cmSaberBack.Disappear(null, 0.01f);
					return;
				}
			}
			else {
				this.cmSaber.Disappear(null, -1f);
				if (setBack) {
					this.cmSaberBack.Appear(null, 0.01f);
				}
			}
		}
	}

	private void ToggleGun(bool enable) {
		if (this.cmGun) {
			if (enable) {
				if (this._refEntity._characterDirection == CharacterDirection.RIGHT) {
					this.cmGun.UpdateTex(0);
				}
				else {
					this.cmGun.UpdateTex(1);
				}
				this.cmGun.Appear(null, -1f);
				return;
			}
			this.cmGun.Disappear(null, -1f);
		}
	}

	public override void ClearSkill() {
		this.isSkillEventEnd = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this.ToggleSaber(false, true);
		this.ToggleGun(false);
		this._refEntity.EnableCurrentWeapon();
		if (this._refEntity.BulletCollider.IsActivate) {
			this._refEntity.BulletCollider.BackToPool();
		}
	}

	public override void SetStun(bool enable) {
		base.SetStun(enable);
		this.ToggleSaber(false, true);
		this.ToggleGun(false);
		this._refEntity.EnableCurrentWeapon();
	}

	public override void ControlCharacterDead() {
		this.ToggleGun(false);
		this.ToggleSaber(false, false);
	}

	public void ChangeComboSkillEvent(Il2CppReferenceArray<Il2CppSystem.Object> parameters) {
		if (parameters.Length != 2) {
			return;
		}
		if (this._refPlayer == null) {
			return;
		}
		bool flag = (int)parameters[0].Unbox<int>() != 0;
		int num = (int)parameters[1].Unbox<int>();
		bool flag2 = this.IsUseStinger();
		if (flag2) {
			this._refPlayer.ForceChangeSkillIcon(1, this._refEntity.PlayerSkills[0].Icon);
		}
		if (!flag) {
			if (flag2) {
				this._refPlayer.SetVirtualButtonAnalog(VirtualButtonId.SKILL0, false);
				this._refPlayer.ClearVirtualButtonStick(VirtualButtonId.SKILL0);
				return;
			}
			this._refPlayer.SetVirtualButtonAnalog(VirtualButtonId.SKILL0, true);
		}
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray (
			new string[] {
				"ch140_skill_01_phase1_start",
				"ch140_skill_01_phase1_loop",
				"ch140_skill_01_phase1_jump_end",
				"ch140_skill_01_phase2_stand",
				"ch140_skill_01_phase2_jump",
				"ch140_skill_02_jump_start",
				"ch140_skill_02_jump_loop",
				"ch140_skill_02_jump_end"
			}
		);
	}

	public override Il2CppStringArray GetCharacterDependBlendAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch140_skill_01_phase1_start",
				"ch140_skill_01_phase1_loop"
			}
		);
	}

	private int nowFrame;

	private int skillEventFrame;

	private int endFrame;

	private int endBreakFrame;

	private bool isSkillEventEnd;

	private int comboId = 1512;

	private OrangeTimer NOVASTRIKETimer;

	private bool isFrenzyStatus;

	private int conditionId = 1513;

	private ParticleSystem[] _FrenzyEffect;

	private ParticleSystem _teleportEffect;

	private bool isInit;

	private Vector3 shootDirection = Vector3.right;

	private CharacterMaterial cmGun;

	private CharacterMaterial cmSaber;

	private CharacterMaterial cmSaberBack;

	// [SerializeField]
	private int risingSpdY = 12000;

	private OrangeConsoleCharacter _refPlayer;

	private int teleportFxFrame = -1;

	private bool isWinPose;

	private readonly string FX_0_00 = "fxuse_DMCXStinger_000";

	private readonly string FX_0_01 = "fxuse_DMCXOverDrive_000";

	private readonly string FX_1_00 = "fxuse_DMCXRainStorm_000";

	private readonly string FX_1_01 = "fxuse_DMCXRainStorm_001";

	private readonly string FX_2_00 = "fxuse_DMCXDevilTrigger_000";

	protected readonly int SKL0_1_TRIGGER = (int)(0.09f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_2_TRIGGER = (int)(0.29f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_3_TRIGGER = (int)(0.49f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_END = (int)(1f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL0_END_BREAK = (int)(0.825f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_TRIGGER = (int)(0.25f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_LOOP = (int)(1.75f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_END = (int)(0.667f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int SKL1_END_BREAK = (int)(0.6f / GameLogicUpdateManager.m_fFrameLen);

	protected readonly int LOGOUT_TRIGGER = (int)(0.73f / GameLogicUpdateManager.m_fFrameLen);

	private enum GunFacing {
			Right,
			Left
	}
}
