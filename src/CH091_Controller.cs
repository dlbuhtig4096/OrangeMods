using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH091_Controller : CharacterControlBase {
	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
		this.InitEnhanceSkill();
	}

	private void InitializeSkill() {
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_EX3_0, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse000_EX3_1, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse001_0, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.sFxuse001_1, 1, null);
		Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		this.shootPointTransform = new GameObject(this.sCustomShootPoint).transform;
		this.shootPointTransform.SetParent(base.transform);
		this.shootPointTransform.localPosition = new Vector3(0f, 0.85f, 0f);
		this._refEntity.PlayerSkills[0].ShootTransform[0] = this.shootPointTransform;
		this.shootPointTransform2 = new GameObject(this.sCustomShootPoint + "2").transform;
		this.shootPointTransform2.SetParent(OrangeBattleUtility.FindChildRecursive(componentsInChildren, this.sBipProp1, true));
		this.shootPointTransform2.localPosition = new Vector3(0f, 0f, 1.6f);
		this.shootPointTransform2.transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, -90f));
		this.shootPointTransform2.transform.localScale = new Vector3(1f, 1f, 1f);
		this.shootPointTransform3 = new GameObject(this.sCustomShootPoint + "3").transform;
		this.shootPointTransform3.SetParent(base.transform);
		this.shootPointTransform3.localPosition = new Vector3(0f, 0f, 0f);
		this.shootPointTransform4 = new GameObject(this.sCustomShootPoint + "4").transform;
		this.shootPointTransform4.SetParent(this.shootPointTransform3);
		this.shootPointTransform4.localPosition = new Vector3(0f, 0f, 0f);
		this.WeaponMesh_c = OrangeBattleUtility.FindChildRecursive(componentsInChildren, this.sWeaponMesh_c, true).gameObject;
		this.WhipMesh_Sub_e = OrangeBattleUtility.FindChildRecursive(componentsInChildren, this.sWhipMesh_Sub_e, true).gameObject;
		this.SKL1_LOOP_FRAME = (int)(this.GetSklTime(1) / GameLogicUpdateManager.m_fFrameLen) - (this.SKL1_0_START_END - this.SKL1_0_START_TRIGGER);
		this.GuardActive = false;
		this.WhipMesh_Sub_e.SetActive(false);
		this._refEntity.PlayerSkills[1].LastUseTimer.SetTime(9999f);
	}

	private float GetSklTime(int idx) {
		return (float)this._refEntity.PlayerSkills[idx].FastBulletDatas[0].n_FIRE_SPEED / 1000f;
	}

	private void InitEnhanceSkill() {
		this._enhanceSlot = this._refEntity.PlayerSkills[0].EnhanceEXIndex;
		int skillId = (new int[] {
			17101,
			17102,
			17103,
			17104
		})[this._enhanceSlot];
		this._refEntity.ReInitSkillStruct(0, skillId, false);
		for (int i = 0; i < this._refEntity.PlayerSkills[0].FastBulletDatas.Length; i++) {
			if (!MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(this._refEntity.PlayerSkills[0].FastBulletDatas[i].s_MODEL)) {
				BulletBase.PreloadBullet<BasicBullet>(this._refEntity.PlayerSkills[0].FastBulletDatas[i]);
			}
		}
		this.SKL0_EX3_LOOP_FRAME = (int)(this.GetSklTime(0) / GameLogicUpdateManager.m_fFrameLen) - this.SKL0_EX3_START_END;
		float f_RANGE = this._refEntity.PlayerSkills[0].FastBulletDatas[0].f_RANGE;
		this.shootPointTransform4.localPosition = new Vector3(f_RANGE + f_RANGE * 0.2f, 0f, 0f);
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
		this._refEntity.TeleportInCharacterDependEvt = (Callback)new Action(this.TeleportInCharacterDepend);
		this._refEntity.TeleportOutCharacterDependEvt = (Callback)new Action(this.TeleportOutCharacterDepend);
		this._refEntity.GuardCalculateEvt = new Func<HurtPassParam, bool>(this.GuardCalculate);
		this._refEntity.GuardHurtEvt = (Callback<HurtPassParam>)new Action<HurtPassParam>(this.GuardHurt);
		this._refEntity.ChangeComboSkillEventEvt = (CallbackObjs)new Action<Il2CppReferenceArray<Il2CppSystem.Object>>(this.ChangeComboSkillEvent);
	}

	public void TeleportInCharacterDepend() {
		if (this._refEntity.CurrentFrame >= 0.9f) {
			this.UpdateCustomWeaponRenderer(false, false);
		}
	}

	public void TeleportOutCharacterDepend() {
		float currentFrame = this._refEntity.CurrentFrame;
		HumanBase.AnimateId animateID = this._refEntity.AnimateID;
		if (animateID != HumanBase.AnimateId.ANI_WIN_POSE) {
			if (currentFrame >= 0.95f) {
				this.WhipMesh_Sub_e.SetActive(false);
				return;
			}
			if (currentFrame >= 0.45f) {
				this.WhipMesh_Sub_e.SetActive(true);
			}
		}
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id != 0 && id == 1 && this._refEntity.PlayerSkills[id].Reload_index == 0) {
			if (!this._refEntity.CheckUseSkillKeyTriggerEX2(id)) {
				return;
			}
			this.SetGuardInactive();
			base.PlayVoiceSE("v_mh2_skill01_1");
			this._refEntity.CurrentActiveSkill = id;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_0_START_TRIGGER, this.SKL1_0_START_END, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
			this.UpdateAnalog(true);
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id != 0) {
			if (id != 1) {
				return;
			}
			int reload_index = this._refEntity.PlayerSkills[id].Reload_index;
			if (reload_index == 1) {
				if (!this._refEntity.CheckUseSkillKeyTriggerEX2(id)) {
					return;
				}
				this.SetGuardInactive();
				base.PlayVoiceSE("v_mh2_skill01_2");
				this._refEntity.CurrentActiveSkill = id;
				this._refEntity.IsShoot = 1;
				ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_1_TRIGGER, this.SKL1_1_OFFSET_START, OrangeCharacter.SubStatus.SKILL1_3, out this.skillEventFrame, out this.endFrame);
				WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
				this._refEntity.CheckUsePassiveSkill(this._refEntity.CurrentActiveSkill, currentSkillObj.weaponStatus, currentSkillObj.ShootTransform[this._refEntity.CurrentActiveSkill], default(Il2CppSystem.Nullable_Unboxed<Vector3>), new Il2CppSystem.Nullable_Unboxed<int>(currentSkillObj.Reload_index));
				this.UpdateAnalog(false);
			}
			return;
		}
		else {
			if (!this._refEntity.CheckUseSkillKeyTriggerEX2(id)) {
				return;
			}
			OrangeCharacter.SubStatus p_nextStatus = OrangeCharacter.SubStatus.SKILL0;
			int p_sklTriggerFrame = this.SKL0_TRIGGER;
			int p_endFrame = this.SKL0_END;
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
			switch (this._enhanceSlot) {
			case 1:
				p_nextStatus = OrangeCharacter.SubStatus.SKILL0_1;
				break;
			case 2:
				p_nextStatus = OrangeCharacter.SubStatus.SKILL0_2;
				break;
			case 3:
				p_nextStatus = OrangeCharacter.SubStatus.SKILL0_3;
				p_sklTriggerFrame = this.SKL0_EX3_START_END;
				p_endFrame = this.SKL0_EX3_START_END;
				break;
			}
			base.PlayVoiceSE("v_mh2_skill02");
			this._refEntity.CurrentActiveSkill = id;
			this._refEntity.IsShoot = 1;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, p_sklTriggerFrame, p_endFrame, p_nextStatus, out this.skillEventFrame, out this.endFrame);
			return;
		}
	}

	private void UpdateAnalog(bool active) {
		if (this._refEntity is OrangeConsoleCharacter) {
			(this._refEntity as OrangeConsoleCharacter).SetVirtualButtonAnalog(VirtualButtonId.SKILL1, active);
		}
	}

	public void SetStatusCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
		if (mainStatus == OrangeCharacter.MainStatus.SKILL) {
			switch (subStatus) {
			case OrangeCharacter.SubStatus.SKILL0:
				this._refEntity.FreshBullet = true;
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, true);
				this.UpdateCustomWeaponRenderer(true, true);
				return;
			case OrangeCharacter.SubStatus.SKILL0_1:
				this._refEntity.FreshBullet = true;
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, true);
				this.UpdateCustomWeaponRenderer(true, true);
				return;
			case OrangeCharacter.SubStatus.SKILL0_2:
				this._refEntity.FreshBullet = true;
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, HumanBase.AnimateId.ANI_BTSKILL_START, true);
				this.UpdateCustomWeaponRenderer(true, true);
				return;
			case OrangeCharacter.SubStatus.SKILL0_3:
				this._refEntity.FreshBullet = true;
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)129U, (HumanBase.AnimateId)129U, (HumanBase.AnimateId)129U, true);
				this.UpdateCustomWeaponRenderer(true, true);
				return;
			case OrangeCharacter.SubStatus.SKILL0_4:
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)130U, (HumanBase.AnimateId)130U, (HumanBase.AnimateId)130U, true);
				this.chargeFx = FxManager_.PlayReturn<FxBase>(this.sFxuse000_EX3_0, this.shootPointTransform2, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
				return;
			case OrangeCharacter.SubStatus.SKILL0_5:
				FxManager_.Play(this.sFxuse000_EX3_1, this.shootPointTransform2.position, this.shootPointTransform2.rotation);
				ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)131U, (HumanBase.AnimateId)131U, (HumanBase.AnimateId)131U, true);
				return;
			default:
				switch (subStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, HumanBase.AnimateId.ANI_SKILL_START, HumanBase.AnimateId.ANI_SKILL_START, true);
					this.UpdateCustomWeaponRenderer(true, true);
					this.counterFx = FxManager_.PlayReturn<FxBase>(this.sFxuse001_0, this._refEntity.ModelTransform.position, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
					return;
				case OrangeCharacter.SubStatus.SKILL1_1:
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)66U, true);
					return;
				case OrangeCharacter.SubStatus.SKILL1_2:
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)67U, (HumanBase.AnimateId)67U, (HumanBase.AnimateId)67U, true);
					return;
				case OrangeCharacter.SubStatus.SKILL1_3:
					this._refEntity.FreshBullet = true;
					ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)128U, (HumanBase.AnimateId)128U, (HumanBase.AnimateId)128U, true);
					this.UpdateCustomWeaponRenderer(true, true);
					FxManager_.Play(this.sFxuse001_1, this.shootPointTransform2, OrangeBattleUtility.QuaternionNormal);
					break;
				default:
					return;
				}
				break;
			}
		}
	}

	public override void ClearSkill() {
		this.UpdateCustomWeaponRenderer(false, false);
		this._refEntity.EnableCurrentWeapon();
		this.isSkillEventEnd = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this.SetGuardInactive();
	}

	public override void SetStun(bool enable) {
		if (enable) {
			this.UpdateCustomWeaponRenderer(false, false);
			this._refEntity.EnableCurrentWeapon();
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
			case OrangeCharacter.SubStatus.SKILL0_1:
			case OrangeCharacter.SubStatus.SKILL0_2:
				if (this.nowFrame >= this.endFrame) {
					this._refEntity.CurrentActiveSkill = -1;
					this.OnSkillEnd();
					return;
				}
				if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
					this.isSkillEventEnd = true;
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this.shootPointTransform, MagazineType.ENERGY, -1, 1, true);
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
					this.endFrame = this.nowFrame + 1;
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_3:
				if (this.nowFrame >= this.endFrame) {
					this._refEntity.IsShoot = 1;
					base.PlaySkillSE("mh2_tama05");
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 0, this.SKL0_EX3_LOOP_FRAME, this.SKL0_EX3_LOOP_FRAME, OrangeCharacter.SubStatus.SKILL0_4, out this.skillEventFrame, out this.endFrame);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_4:
				if (this.nowFrame >= this.endFrame) {
					if (this.chargeFx != null && this.chargeFx.gameObject.activeSelf) {
						this.chargeFx.BackToPool();
						this.chargeFx = null;
					}
					this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_EX3_END_BREAK;
					ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 0, this.SKL0_EX3_END_END, this.SKL0_EX3_END_END, OrangeCharacter.SubStatus.SKILL0_5, out this.skillEventFrame, out this.endFrame);
					this.isSkillEventEnd = true;
					Vector2.SignedAngle(Vector2.right, this._refEntity.ShootDirection);
					this.shootPointTransform3.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, this._refEntity.ShootDirection));
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this.shootPointTransform4, MagazineType.ENERGY, -1, 0, true);
					this.SetRecoil(0.1f);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_5:
				if (this.nowFrame >= this.endFrame) {
					this._refEntity.CurrentActiveSkill = -1;
					this.OnSkillEnd();
					return;
				}
				if (this.nowFrame >= this.endBreakFrame) {
					this.SetRecoil(0f);
					if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT)) {
						this.endFrame = this.nowFrame + 1;
						return;
					}
				}
				break;
			default:
				switch (curSubStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
					if (this.nowFrame >= this.endFrame) {
						this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_0_LOOP_BREAK;
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 1, this.SKL1_0_START_TRIGGER, this.SKL1_LOOP_FRAME, OrangeCharacter.SubStatus.SKILL1_1, out this.skillEventFrame, out this.endFrame);
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
						int reload_index = currentSkillObj.Reload_index;
						this._refEntity.CheckUsePassiveSkill(this._refEntity.CurrentActiveSkill, currentSkillObj.weaponStatus, base.transform, default(Il2CppSystem.Nullable_Unboxed<Vector3>), new Il2CppSystem.Nullable_Unboxed<int>(reload_index));
						this.GuardActive = true;
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_1:
					if (this.nowFrame >= this.endFrame) {
						this.SetGuardInactive();
						int p_endFrame = this.SKL1_0_END_END;
						if (this._refEntity.IgnoreGravity) {
							p_endFrame = this.SKL1_0_END_END_AIR;
						}
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 1, this.SKL1_0_START_TRIGGER, p_endFrame, OrangeCharacter.SubStatus.SKILL1_2, out this.skillEventFrame, out this.endFrame);
						return;
					}
					if (this.nowFrame >= this.endBreakFrame) {
						this.ChkCounterStatus();
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_2:
					if (this.nowFrame >= this.endFrame) {
						this.OnSkillEnd();
						return;
					}
					this.ChkCounterStatus();
					return;
				case OrangeCharacter.SubStatus.SKILL1_3:
					if (this.nowFrame >= this.endFrame) {
						this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_1_END_BREAK - this.SKL1_1_OFFSET_START;
						ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 1, this.SKL1_1_OFFSET_END - this.SKL1_1_OFFSET_START, this.SKL1_1_END - this.SKL1_1_OFFSET_START, OrangeCharacter.SubStatus.SKILL1_4, out this.skillEventFrame, out this.endFrame);
						this.isSkillEventEnd = false;
						this.SetRecoil(0.8f);
						if (this._refEntity.IgnoreGravity) {
							this._refEntity.IgnoreGravity = false;
							return;
						}
					}
					else if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						WeaponStruct currentSkillObj2 = this._refEntity.GetCurrentSkillObj();
						int reload_index2 = currentSkillObj2.Reload_index;
						ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this.shootPointTransform2, MagazineType.ENERGY, reload_index2, 1, false);
						ComboCheckData[] comboCheckDatas = currentSkillObj2.ComboCheckDatas;
						for (int i = 0; i < comboCheckDatas.Length; i++) {
							this._refEntity.RemoveComboSkillBuff(comboCheckDatas[i].nComboSkillID);
						}
						currentSkillObj2.Reload_index = 0;
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_4:
					if (this.nowFrame >= this.endFrame) {
						this._refEntity.CurrentActiveSkill = -1;
						this.OnSkillEnd();
						return;
					}
					if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
						this.isSkillEventEnd = true;
						this.SetRecoil(0f);
						return;
					}
					if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
						this.endFrame = this.nowFrame + 1;
					}
					break;
				default:
					return;
				}
				break;
			}
		}
	}

	private void ChkCounterStatus() {
		if (ManagedSingleton<InputStorage>.Instance.IsReleased(this._refEntity.UserID, ButtonId.SKILL0)) {
			if (this._refEntity.CanPlayerPressSkill(0, false)) {
				this.SetGuardInactive();
				this.isSkillEventEnd = false;
				this._refEntity.CurrentActiveSkill = -1;
				this._refEntity.PlayerReleaseSkill(0);
				return;
			}
		}
		else if (ManagedSingleton<InputStorage>.Instance.IsReleased(this._refEntity.UserID, ButtonId.SKILL1)) {
			this.isSkillEventEnd = false;
			this._refEntity.CurrentActiveSkill = -1;
			this._refEntity.PlayerReleaseSkill(1);
			return;
		}
		if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.DOWN)) {
			if (ManagedSingleton<InputStorage>.Instance.IsPressed(this._refEntity.UserID, ButtonId.SHOOT)) {
				this.OnSkillEnd();
			}
			return;
		}
		if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT) || ManagedSingleton<InputStorage>.Instance.IsPressed(this._refEntity.UserID, ButtonId.SHOOT)) {
			this.OnSkillEnd();
		}
	}

	private void SetRecoil(float rate) {
		this._refEntity.SetHorizontalSpeed(Mathf.RoundToInt((float)(OrangeCharacter.WalkSpeed * this._refEntity.direction * -1) * rate));
	}

	private void OnSkillEnd() {
		this._refEntity.IgnoreGravity = false;
		this.isSkillEventEnd = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this.UpdateCustomWeaponRenderer(false, false);
		this._refEntity.EnableCurrentWeapon();
		this.SetGuardInactive();
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

	private void UpdateCustomWeaponRenderer(bool enableWeapon, bool enableTrail = false) {
		this.WeaponMesh_c.SetActive(enableWeapon);
	}

	private void SetGuardInactive() {
		this.GuardActive = false;
		PerBuffManager selfBuffManager = this._refEntity.selfBuffManager;
		foreach (int conditionid in this.arrGuardCondtion) {
			selfBuffManager.RemoveBuffByCONDITIONID(conditionid, true);
		}
		if (this.counterFx != null && this.counterFx.gameObject.activeSelf) {
			this.counterFx.BackToPool();
			this.counterFx = null;
		}
	}

	public override bool GuardCalculate(HurtPassParam tHurtPassParam) {
		return this._refEntity.Hp > 0 && this.GuardActive;
	}

	public void GuardHurt(HurtPassParam tHurtPassParam) {
		CodeStage.AntiCheat.ObscuredTypes.ObscuredInt dmg = 0;
		if (this._refEntity.IsLocalPlayer) {
			PerBuffManager selfBuffManager = this._refEntity.selfBuffManager;
			this._refEntity.tRefPassiveskill.HurtTrigger(ref dmg, this._refEntity.GetCurrentWeaponObj().weaponStatus.nWeaponCheck, ref selfBuffManager, new Action<SKILL_TABLE>(this._refEntity.CreateBulletByLastWSTranform));
		}
		tHurtPassParam.dmg = dmg;
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch091_skill_02_step1_start",
				"ch091_skill_02_step1_loop",
				"ch091_skill_02_step1_end"
			}
		);
	}

	public override Il2CppReferenceArray<Il2CppStringArray> GetCharacterDependAnimationsBlendTree() {
		return new Il2CppReferenceArray<Il2CppStringArray> (
			new Il2CppStringArray[] {
				new Il2CppStringArray(
					new string[] {
						"ch091_skill_01_up",
						"ch091_skill_01_mid",
						"ch091_skill_01_down"
					}
				),
				new Il2CppStringArray(
					new string[] {
						"ch091_skill_02_step2_shot_up",
						"ch091_skill_02_step2_shot_mid",
						"ch091_skill_02_step2_shot_down"
					}
				),
				new Il2CppStringArray(
					new string[] {
						"ch091_skill_01_charging_up_start",
						"ch091_skill_01_charging_mid_start",
						"ch091_skill_01_charging_down_start"
					}
				),
				new Il2CppStringArray(
					new string[] {
						"ch091_skill_01_charging_up_loop",
						"ch091_skill_01_charging_mid_loop",
						"ch091_skill_01_charging_down_loop"
					}
				),
				new Il2CppStringArray(
					new string[] {
						"ch091_skill_01_charging_up_end",
						"ch091_skill_01_charging_mid_end",
						"ch091_skill_01_charging_down_end"
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

	private int SKL0_EX3_LOOP_FRAME;

	private int SKL1_LOOP_FRAME;

	private bool GuardActive;

	private Transform shootPointTransform;

	private Transform shootPointTransform2;

	private Transform shootPointTransform3;

	private Transform shootPointTransform4;

	private GameObject WeaponMesh_c;

	private GameObject WhipMesh_Sub_e;

	private FxBase chargeFx;

	private FxBase counterFx;

	private readonly int[] arrGuardCondtion = new int[] {
		1205,
		1206
	};

	protected int _enhanceSlot;

	private readonly string sFxuse000_EX3_0 = "fxuse_bowgun_003";

	private readonly string sFxuse000_EX3_1 = "fxuse_bowgun_002";

	private readonly string sFxuse001_0 = "fxuse_countershot_000";

	private readonly string sFxuse001_1 = "fxuse_countershot_001";

	private readonly string sCustomShootPoint = "CustomShootPoint";

	private readonly string sBipProp1 = "Bip Prop1";

	private readonly string sWeaponMesh_c = "WeaponMesh_c";

	private readonly string sWhipMesh_Sub_e = "WhipMesh_Sub_e";

	private readonly int SKL0_TRIGGER = (int)(0.18207f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END = (int)(0.867f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END_BREAK = (int)(0.36f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_EX3_START_END = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_EX3_END_END = (int)(0.667f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_EX3_END_BREAK = (int)(0.28f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_0_START_TRIGGER = 1;

	private readonly int SKL1_0_START_END = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_0_LOOP_BREAK = 1;

	private readonly int SKL1_0_END_END = (int)(0.667f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_0_END_END_AIR = (int)(0.222f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_TRIGGER = (int)(0.323793f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_END = (int)(1.133f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_END_BREAK = (int)(0.83f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_OFFSET_START = (int)(0.37389f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_1_OFFSET_END = (int)(0.81576f / GameLogicUpdateManager.m_fFrameLen);
}
