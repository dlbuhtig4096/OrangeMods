using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH099_Controller : CharacterControlBase {
	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
	}

	private void InitializeSkill() {
		this.shootPointTransform0 = new GameObject(this.sCustomShootPoint + "0").transform;
		this.shootPointTransform0.SetParent(base.transform);
		this.shootPointTransform0.localPosition = new Vector3(0f, 0.8f, 0f);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_000, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse001_001, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse001_002, 1, null);
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.CheckSkillLockDirectionEvt = (Callback)new Action(this.CheckSkillLockDirection);
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
			base.PlayVoiceSE("v_ch_skill02");
			base.PlaySkillSE("ch_hoyokusen");
			this._refEntity.CurrentActiveSkill = id;
			ManagedSingleton<CharacterControlHelper>.Instance.TurnToAimTarget(this._refEntity);
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_TRIGGER, this.SKL1_END, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)71U, (HumanBase.AnimateId)71U, (HumanBase.AnimateId)71U, true);
			this.SetKickMovement(-0.2f);
			return;
		}
		else {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			base.PlayVoiceSE("v_ch_skill01");
			base.PlaySkillSE("ch_spining_lg");
			this._refEntity.SoundSource.PlaySE("SkillSE_CHUNLI", "ch_spining_stop", 0.8f);
			this.skl1Direction = this._refEntity._characterDirection;
			this._refEntity.CurrentActiveSkill = id;
			this._refEntity.IsShoot = 0;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_START_END, this.SKL0_START_END, OrangeCharacter.SubStatus.SKILL0, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, HumanBase.AnimateId.ANI_SKILL_START, (HumanBase.AnimateId)68U, true);
			if (this._refEntity.IgnoreGravity) {
				this.offsetY = -1;
				return;
			}
			this.offsetY = this._refEntity.Controller.LogicPosition.y;
			return;
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
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
					if (this._refEntity.Dashing) {
						this._refEntity.PlayerStopDashing();
					}
					WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
					SKILL_TABLE bulletData = currentSkillObj.BulletData;
					BulletBase bulletCollider = this._refEntity.BulletCollider;
					SKILL_TABLE pData = bulletData;
					string sPlayerName = this._refEntity.sPlayerName;
					int nowRecordNO = this._refEntity.GetNowRecordNO();
					OrangeCharacter refEntity = this._refEntity;
					int nBulletRecordID = refEntity.nBulletRecordID;
					refEntity.nBulletRecordID = nBulletRecordID + 1;
					bulletCollider.UpdateBulletData(pData, sPlayerName, nowRecordNO, nBulletRecordID, 1);
					this._refEntity.BulletCollider.SetBulletAtk(currentSkillObj.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
					this._refEntity.BulletCollider.BulletLevel = currentSkillObj.SkillLV;
					this._refEntity.BulletCollider.Active(this._refEntity.TargetMask);
					this._refEntity.CheckUsePassiveSkill(this._refEntity.CurrentActiveSkill, currentSkillObj.weaponStatus, currentSkillObj.ShootTransform[this._refEntity.CurrentActiveSkill]);
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_LOOP_END, this.SKL0_LOOP_END, OrangeCharacter.SubStatus.SKILL0_1, out this.skillEventFrame, out this.endFrame);
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)69U, true);
					// this._refEntity.SetSpeed((int)(this.skl1Direction * (CharacterDirection)((float)OrangeCharacter.DashSpeed * 2.5f)), 0);
					this._refEntity.SetSpeed((int)((float)this.skl1Direction * (float)OrangeCharacter.DashSpeed * 2.5f), 0);
					OrangeBattleUtility.UpdateSkillCD(currentSkillObj);
					FxManager_.Play(this.sFxuse000_000, this._refEntity.ModelTransform, Quaternion.identity);
					this._refEntity.IgnoreGravity = true;
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_1:
				if (this.nowFrame >= this.endFrame) {
					this._refEntity.SetSpeed(0, 0);
					this._refEntity.BulletCollider.BackToPool();
					int num = this._refEntity.IsInGround ? 1 : this.SKL0_END_END;
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, num, num, OrangeCharacter.SubStatus.SKILL0_2, out this.skillEventFrame, out this.endFrame);
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)67U, (HumanBase.AnimateId)67U, (HumanBase.AnimateId)70U, true);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_2:
				if (this.nowFrame >= this.endFrame) {
					this.endBreakFrame = GameLogicUpdateManager.GameFrame;
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL0_END_BREAK, this.SKL0_END_BREAK, OrangeCharacter.SubStatus.SKILL0_3, out this.skillEventFrame, out this.endFrame);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_3:
				if (this.nowFrame >= this.endFrame) {
					this.OnSkillEnd();
					return;
				}
				if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT)) {
					this.endFrame = this.nowFrame + 1;
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
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						this.SetKickMovement(0f);
						FxManager_.Play(this.sFxuse001_002, this._refEntity.ModelTransform.position, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
						return;
					}
					if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
						this.endFrame = this.nowFrame + 1;
					}
				}
				else {
					if (this.nowFrame >= this.endFrame) {
						this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_1_END_BREAK;
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 0, this.SKL1_1_TRIGGER, this.SKL1_1_END, OrangeCharacter.SubStatus.SKILL1_1, out this.skillEventFrame, out this.endFrame);
						ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)72U, (HumanBase.AnimateId)72U, (HumanBase.AnimateId)72U, true);
						this.SetKickMovement(0.2f);
						this.isSkillEventEnd = false;
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.SetKickMovement(0f);
						this.isSkillEventEnd = true;
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this.shootPointTransform0, MagazineType.ENERGY, -1, 0, true);
						FxManager_.Play(this.sFxuse001_001, this._refEntity.ModelTransform.position, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
						return;
					}
				}
				break;
			}
		}
	}

	private void SetKickMovement(float rate) {
		this._refEntity.SetHorizontalSpeed(Mathf.RoundToInt((float)(OrangeCharacter.WalkSpeed * (int)this.skl1Direction) * rate));
	}

	private void OnSkillEnd() {
		if (this._refEntity.IgnoreGravity) {
			this._refEntity.IgnoreGravity = false;
		}
		bool flag = this._refEntity.CurrentActiveSkill == 0 && this.offsetY == this._refEntity.Controller.LogicPosition.y;
		this.isSkillEventEnd = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.EnableCurrentWeapon();
		HumanBase.AnimateId animateID = this._refEntity.AnimateID;
		if (animateID - HumanBase.AnimateId.ANI_SKILL_START <= 2U || animateID - (HumanBase.AnimateId)71U <= 1U) {
			this._refEntity.Dashing = false;
			this._refEntity.SetSpeed(0, 0);
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
			return;
		}
		if (flag) {
			this._refEntity.Dashing = false;
			this._refEntity.SetSpeed(0, 0);
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
			return;
		}
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
	}

	public override void ClearSkill() {
		this._refEntity.EnableCurrentWeapon();
		this.isSkillEventEnd = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.BulletCollider.BackToPool();
	}

	public override void SetStun(bool enable) {
		if (enable) {
			this._refEntity.EnableCurrentWeapon();
		}
	}

	public void CheckSkillLockDirection() {
		OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
		if (curSubStatus - OrangeCharacter.SubStatus.SKILL0 <= 3) {
			this._refEntity._characterDirection = this.skl1Direction;
			return;
		}
		// this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
		this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch099_skill_01_start",
				"ch099_skill_01_loop",
				"ch099_skill_01_end",
				"ch099_skill_01_start",
				"ch099_skill_01_loop",
				"ch099_skill_01_end",
				"ch099_skill_02_step1",
				"ch099_skill_02_step2"
			}
		);
	}

	private int nowFrame;

	private int skillEventFrame;

	private int endFrame;

	private bool isSkillEventEnd;

	private int endBreakFrame;

	private Transform shootPointTransform0;

	private CharacterDirection skl1Direction = CharacterDirection.LEFT;

	private int offsetY;

	private readonly string sFxuse000_000 = "fxuse_spiningbirdkick_000";

	private readonly string sFxuse001_001 = "fxuse_hoyokusen_001";

	private readonly string sFxuse001_002 = "fxuse_hoyokusen_002";

	private readonly string sCustomShootPoint = "CustomShootPoint";

	private readonly int SKL0_START_END = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_LOOP_END = (int)(0.417f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END_END = (int)(0.156f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END_BREAK = (int)(0.4f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_TRIGGER = (int)(0.08f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_END = (int)(0.43f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_TRIGGER = (int)(0.08f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_END = (int)(0.33f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_END_BREAK = (int)(0.16f / GameLogicUpdateManager.m_fFrameLen);
}
