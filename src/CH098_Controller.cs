using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH098_Controller : CharacterControlBase {
	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
	}

	private void InitializeSkill() {
		Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		Transform transform = new GameObject(this.sCustomShootPoint + "1").transform;
		transform.SetParent(base.transform);
		transform.localPosition = new Vector3(0f, 0f, 0f);
		Transform transform2 = new GameObject(this.sCustomShootPoint + "2").transform;
		transform2.SetParent(base.transform);
		transform2.localPosition = new Vector3(2f, 0f, 0f);
		this._refEntity.ExtraTransforms = new Transform[] {
			OrangeBattleUtility.FindChildRecursive(componentsInChildren, "L WeaponPoint", true),
			transform,
			transform2
		};
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_000, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_001, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_002, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_003, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse001_000, 1, null);
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.ChangeComboSkillEventEvt = (CallbackObjs)new Action<Il2CppReferenceArray<Il2CppSystem.Object>>(this.ChangeComboSkillEvent);
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 1) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			base.PlayVoiceSE("v_ry_skill02");
			base.PlaySkillSE("ry_denzinrenki");
			this._refEntity.CurrentActiveSkill = id;
			this._refEntity.IsShoot = 1;
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_TRIGGER, this.SKL1_END, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, HumanBase.AnimateId.ANI_SKILL_START, HumanBase.AnimateId.ANI_SKILL_START, true);
			FxManager_.Play(this.sFxuse001_000, this._refEntity.ExtraTransforms[1].position, OrangeBattleUtility.QuaternionNormal);
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 0) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this._refEntity.CurrentActiveSkill = id;
			this._refEntity.IsShoot = 1;
			int reload_index = this._refEntity.GetCurrentSkillObj().Reload_index;
			string empty = string.Empty;
			OrangeCharacter.SubStatus p_nextStatus;
			if (reload_index == 0 || reload_index != 1) {
				p_nextStatus = OrangeCharacter.SubStatus.SKILL0;
				empty = this.sFxuse000_000;
				base.PlayVoiceSE("v_ry_skill01_1");
				base.PlaySkillSE("ry_shinkuhado");
			}
			else {
				p_nextStatus = OrangeCharacter.SubStatus.SKILL0_1;
				empty = this.sFxuse000_002;
				base.PlayVoiceSE("v_ry_skill01_2");
				base.PlaySkillSE("ry_denzinhado");
			}
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_TRIGGER, this.SKL0_END, p_nextStatus, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, true);
			FxManager_.Play(empty, this._refEntity.ExtraTransforms[0], Quaternion.identity);
		}
	}

	public override void CheckSkill() {
		if (this._refEntity.IsLocalPlayer && this._refEntity.PlayerSkills[0].Reload_index > 0 && !this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(this.conditionId, 0)) {
			this.ResetComboIndex(this._refEntity.PlayerSkills[0]);
		}
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
			if (curSubStatus != OrangeCharacter.SubStatus.SKILL0) {
				if (curSubStatus != OrangeCharacter.SubStatus.SKILL0_1) {
					if (curSubStatus != OrangeCharacter.SubStatus.SKILL1) {
						return;
					}
					if (this.nowFrame >= this.endFrame) {
						this._refEntity.CurrentActiveSkill = -1;
						this.OnSkillEnd();
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						int reload_index = this._refEntity.GetCurrentSkillObj().Reload_index;
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ExtraTransforms[1], MagazineType.ENERGY, reload_index, 0, true);
						return;
					}
					if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
						this.endFrame = this.nowFrame + 1;
					}
				}
				else {
					if (this.nowFrame >= this.endFrame) {
						this._refEntity.CurrentActiveSkill = -1;
						this.OnSkillEnd();
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
						int reload_index2 = currentSkillObj.Reload_index;
						currentSkillObj.ShootTransform[0] = this._refEntity.ExtraTransforms[0];
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ExtraTransforms[0], MagazineType.ENERGY, reload_index2, 0, false);
						this._refEntity.CheckUsePassiveSkill(0, currentSkillObj.weaponStatus, currentSkillObj.ShootTransform[0], default(Il2CppSystem.Nullable_Unboxed<Vector3>), new Il2CppSystem.Nullable_Unboxed<int>(reload_index2));
						this._refEntity.ExtraTransforms[2].localPosition = this.GetGxOffset();
						ManagedSingleton<CharacterControlHelper>.Instance.Play360ShootEft(this._refEntity, this.sFxuse000_003, this._refEntity.ExtraTransforms[2].position);
						return;
					}
					if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
						this.endFrame = this.nowFrame + 1;
						return;
					}
				}
			}
			else {
				if (this.nowFrame >= this.endFrame) {
					this._refEntity.CurrentActiveSkill = -1;
					this.OnSkillEnd();
					return;
				}
				if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
					this.isSkillEventEnd = true;
					WeaponStruct currentSkillObj2 = this._refEntity.GetCurrentSkillObj();
					int reload_index3 = currentSkillObj2.Reload_index;
					currentSkillObj2.ShootTransform[0] = this._refEntity.ExtraTransforms[0];
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ExtraTransforms[0], MagazineType.ENERGY, reload_index3, 0, false);
					this._refEntity.CheckUsePassiveSkill(0, currentSkillObj2.weaponStatus, currentSkillObj2.ShootTransform[0], default(Il2CppSystem.Nullable_Unboxed<Vector3>), new Il2CppSystem.Nullable_Unboxed<int>(reload_index3));
					this._refEntity.ExtraTransforms[2].localPosition = this.GetGxOffset();
					ManagedSingleton<CharacterControlHelper>.Instance.Play360ShootEft(this._refEntity, this.sFxuse000_001, this._refEntity.ExtraTransforms[2].position);
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
					this.endFrame = this.nowFrame + 1;
					return;
				}
			}
		}
	}

	private Vector3 GetGxOffset() {
		Vector3 vector = this._refEntity.ModelTransform.InverseTransformPoint(this._refEntity.ExtraTransforms[0].position);
		return new Vector3((float)this._refEntity.direction * vector.x, vector.y, vector.z);
	}

	private void ResetComboIndex(WeaponStruct currentSkill) {
		ComboCheckData[] comboCheckDatas = currentSkill.ComboCheckDatas;
		for (int i = 0; i < comboCheckDatas.Length; i++) {
			this._refEntity.RemoveComboSkillBuff(comboCheckDatas[i].nComboSkillID);
		}
		currentSkill.Reload_index = 0;
	}

	private void OnSkillEnd() {
		this._refEntity.IgnoreGravity = false;
		this.isSkillEventEnd = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.EnableCurrentWeapon();
		if (this._refEntity.Controller.Collisions.below) {
			this._refEntity.Dashing = false;
			this._refEntity.PlayerStopDashing();
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
			return;
		}
		if (this._refEntity.IgnoreGravity) {
			this._refEntity.IgnoreGravity = false;
		}
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
	}

	public override void ClearSkill() {
		this._refEntity.EnableCurrentWeapon();
		this.isSkillEventEnd = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
	}

	public override void SetStun(bool enable) {
		if (enable) {
			this._refEntity.EnableCurrentWeapon();
		}
	}

	public void ChangeComboSkillEvent(Il2CppReferenceArray<Il2CppSystem.Object> parameters) {
		if (parameters.Length != 2) {
			return;
		}
		int num = (int)parameters[0].Unbox<int>();
		int num2 = (int)parameters[1].Unbox<int>();
		if (this._refEntity.CurMainStatus == OrangeCharacter.MainStatus.TELEPORT_IN || this._refEntity.CurMainStatus == OrangeCharacter.MainStatus.TELEPORT_OUT || this._refEntity.Hp <= 0) {
			return;
		}
		if (num == 0 && this._refEntity.PlayerSkills[0].Reload_index != num2) {
			this._refEntity.PlayerSkills[0].Reload_index = num2;
		}
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch098_skill_02_stand"
			}
		);
	}

	public override Il2CppReferenceArray<Il2CppStringArray> GetCharacterDependAnimationsBlendTree() {
		return new Il2CppReferenceArray<Il2CppStringArray>(
			new Il2CppStringArray[] {
				new Il2CppStringArray(
					new string[] {
						"ch098_skill_01_stand_up",
						"ch098_skill_01_stand_mid",
						"ch098_skill_01_stand_down"
					}
				)
			}
		);
	}

	private int nowFrame;

	private int skillEventFrame;

	private int endFrame;

	private bool isSkillEventEnd;

	private int endBreakFrame;

	private readonly int conditionId = 1232;

	private readonly string sFxuse000_000 = "fxuse_charge_hadoken_000";

	private readonly string sFxuse000_001 = "fxuse_hadoken_000";

	private readonly string sFxuse000_002 = "fxuse_charge_hadoken_001";

	private readonly string sFxuse000_003 = "fxuse_hadoken_001";

	private readonly string sFxuse001_000 = "fxuse_denjin_000";

	private readonly string sCustomShootPoint = "CustomShootPoint";

	private readonly int SKL0_TRIGGER = (int)(0.16f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END = (int)(0.833f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END_BREAK = (int)(0.35f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_TRIGGER = (int)(0.35f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_END = (int)(0.867f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_END_BREAK = (int)(0.38f / GameLogicUpdateManager.m_fFrameLen);
}
