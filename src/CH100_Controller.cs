using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH100_ShungokusatsuDummy : CollideBullet {

	public override void Active(Vector3 pPos, Vector3 pDirection, LayerMask pTargetMask, IAimTarget pTarget = null) {
		Plugin.Log.LogWarning("射發這個子彈請用Transform");
		this.CallBase<CollideBullet, Action<Vector3, Vector3, LayerMask, IAimTarget>>("Active", pPos, pDirection, pTargetMask, pTarget); // base.Active(pPos, pDirection, pTargetMask, pTarget);
		this.SyncInfoToOwner(pDirection, pTarget);
	}

	public override void Active(Transform pTransform, Vector3 pDirection, LayerMask pTargetMask, IAimTarget pTarget = null) {
		this.CallBase<CollideBullet, Action<Transform, Vector3, LayerMask, IAimTarget>>("Active", pTransform, pDirection, pTargetMask, pTarget); // base.Active(pTransform, pDirection, pTargetMask, pTarget);
		this._transform.SetParent(pTransform);
		this._transform.localPosition = Vector3.zero;
		this._transform.localRotation = Quaternion.identity;
		this.SyncInfoToOwner(pDirection, pTarget);
	}

	protected void SyncInfoToOwner(Vector3 direction, IAimTarget target) {
		if (this.refPBMShoter == null || this.refPBMShoter.SOB == null) {
			return;
		}
		this._player = (this.refPBMShoter.SOB as OrangeCharacter);
		this._owner = this.refPBMShoter.SOB.GetComponent<CH100_Controller>();
		if (this._player == null || this._owner == null) {
			return;
		}
		if (!this._player.IsLocalPlayer) {
			this._owner.SyncSkillDirection(direction, target);
		}
	}

	// protected override void OnTriggerStay2D(Collider2D col) {
	protected void OnTriggerStay2D(Collider2D col) {
		if (this._player == null) {
			return;
		}
		if (this._player.CurMainStatus == OrangeCharacter.MainStatus.SKILL && this._player.CurSubStatus == OrangeCharacter.SubStatus.SKILL1_1) {
			this.CallBase<CollideBullet, Action<Collider2D>>("OnTriggerStay2D", col); // base.OnTriggerStay2D(col);
		}
	}

	public override void BackToPool() {
		this.CallBase<CollideBullet>("BackToPool"); // base.BackToPool();
		this._player = null;
		this._owner = null;
		MonoBehaviourSingleton<PoolManager>.Instance.BackToPool<PoolBaseObject>(this, this.itemName);
	}

	private OrangeCharacter _player;

	private CH100_Controller _owner;
}

public class CH100_Controller : CharacterControlBase {
	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch100_skill_01_stand",
				"ch100_skill_02_step1_start",
				"ch100_skill_02_step1_loop",
				"ch100_skill_02_step2_loop",
				"ch100_skill_02_step2_end"
			}
		);
	}

	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitLinkSkill();
		this.InitPet();
		this.InitExtraMeshData();
	}

	private void InitExtraMeshData() {
		Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		this._refEntity.ExtraTransforms = new Transform[5];
		this._refEntity.ExtraTransforms[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "L WeaponPoint", true);
		this._refEntity.ExtraTransforms[1] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "R WeaponPoint", true);
		this._refEntity.ExtraTransforms[2] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "Skill0FxPoint", true);
		this._refEntity.ExtraTransforms[3] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "Skill1Point", true);
		this._refEntity.ExtraTransforms[4] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "Skill1Point02", true);
		Transform transform = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxuse_skill", true);
		if (transform != null) {
			this.m_fxuse_skill = transform.GetComponent<ParticleSystem>();
		}
		Transform transform2 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "p_shungokusatsu_000", true);
		this._pShungokusatsuBullet = transform2.GetComponent<CH100_ShungokusatsuBullet>();
		this._otSkill1Timer = OrangeTimerManager.GetTimer(TimerMode.FRAME);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_sekia_000", 3, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_shungokusatsu_000", 3, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_shungokusatsu_001", 3, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_shungokusatsu_003", 3, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_shungokusatsu_004", 3, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxhit_shungokusatsu_000", 5, null);
	}

	public override void ExtraVariableInit() {
		if (this.m_fxuse_skill != null && this.m_fxuse_skill.isPlaying) {
			this.m_fxuse_skill.Stop();
		}
	}

	private void InitLinkSkill() {
		this._refPlayer = (this._refEntity as OrangeConsoleCharacter);
		if (this._tSkill1_LinkSkill == null && this._refEntity.PlayerSkills[1].BulletData.n_LINK_SKILL != 0) {
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
			if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(weaponStruct.BulletData.n_LINK_SKILL, out this._tSkill1_LinkSkill)) {
				this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this._tSkill1_LinkSkill);
				GameObject gameObject = new GameObject();
				CollideBullet go = gameObject.AddComponent<CollideBullet>();
				gameObject.name = this._tSkill1_LinkSkill.s_MODEL;
				gameObject.layer = ManagedSingleton<OrangeLayerManager>.Instance.BulletLayer;
				MonoBehaviourSingleton<PoolManager>.Instance.CreatePoolBaseLocal<CollideBullet>(go, this._tSkill1_LinkSkill.s_MODEL, 5);
			}
		}
	}

	private void InitPet() {
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.AnimationEndCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.AnimationEndCharacterDepend);
		this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
		this._refEntity.CheckSkillLockDirectionEvt = (Callback)new Action(this.CheckSkillLockDirection);
		this._refEntity.TeleportInExtraEffectEvt = (Callback)new Action(this.TeleportInExtraEffect);
	}

	public override void SyncSkillDirection(Vector3 dir, IAimTarget target) {
		this._bSyncSkillTargetCompleted = true;
		this._vSyncDirection = dir;
		this._pSyncTarget = target;
	}

	public override void ClearSkill() {
		if (this._refEntity.CurrentActiveSkill == 0) {
			this.CancelSkill0();
		}
		else if (this._refEntity.CurrentActiveSkill == 1) {
			this.CancelSkill1();
		}
		this._refEntity.CurrentActiveSkill = -1;
	}

	public override void CheckSkill() {
		if (this._refEntity.IsAnimateIDChanged()) {
			return;
		}
		this.UpdateVirtualButtonAnalog();
		this.UpdateSkill();
	}

	private void UpdateVirtualButtonAnalog() {
		if (!this._refEntity.IsLocalPlayer) {
			return;
		}
		if (this._refPlayer != null) {
			this._refPlayer.SetVirtualButtonAnalog(VirtualButtonId.SKILL1, true);
		}
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (this._refEntity.CurrentActiveSkill != -1) {
			return;
		}
		if (id == 0) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			base.PlayVoiceSE("v_go_skill01");
			base.PlaySkillSE("go_spining");
			this.UseSkill0(id);
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
			base.PlayVoiceSE("v_go_skill02");
			base.PlaySkillSE("go_shungokusatsu01");
			this.UseSkill1(id);
		}
	}

	public void SetStatusCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
		if ((mainStatus != OrangeCharacter.MainStatus.SKILL || this._refEntity.CurSubStatus != OrangeCharacter.SubStatus.SKILL0) && this.m_fxuse_skill != null && this.m_fxuse_skill.isPlaying) {
			this.m_fxuse_skill.Stop();
		}
		switch (mainStatus) {
		case OrangeCharacter.MainStatus.TELEPORT_IN:
			return;
		case OrangeCharacter.MainStatus.TELEPORT_OUT:
			if (subStatus == OrangeCharacter.SubStatus.TELEPORT_POSE) {
				this.ToggleWeapon(-3);
				return;
			}
			if (subStatus == OrangeCharacter.SubStatus.WIN_POSE) {
				Vector3 p_worldPos = this._refEntity.ModelTransform.position + new Vector3(1.09f, 0f, 0f) * (float)this._refEntity.direction;
				FxManager_.Play("fxdemo_gouki_000B", p_worldPos, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
				this.ToggleWeapon(-2);
				return;
			}
			break;
		case OrangeCharacter.MainStatus.SLASH:
			break;
		case OrangeCharacter.MainStatus.SKILL:
			if (subStatus == OrangeCharacter.SubStatus.SKILL0) {
				this._refEntity.Animator._animator.speed = 2f;
				this._refEntity.SetAnimateId(HumanBase.AnimateId.ANI_SKILL_START);
				return;
			}
			if (subStatus == OrangeCharacter.SubStatus.SKILL0_1) {
				this._refEntity.IgnoreGravity = true;
				this._refEntity.Animator._animator.speed = 2f;
				this._refEntity.SetAnimateId(HumanBase.AnimateId.ANI_SKILL_START);
				return;
			}
			switch (subStatus) {
			case OrangeCharacter.SubStatus.SKILL1:
				this._refEntity.SetAnimateId((HumanBase.AnimateId)66U);
				this.CreateSkillBullet(this._refEntity.PlayerSkills[1]);
				return;
			case OrangeCharacter.SubStatus.SKILL1_1:
				if (this.m_fxuse_skill != null && !this.m_fxuse_skill.isPlaying) {
					this.m_fxuse_skill.Play();
				}
				this._refEntity.SetAnimateId((HumanBase.AnimateId)67U);
				this._refEntity.SetSpeed((int)this._vSkillVelocity.x, (int)this._vSkillVelocity.y);
				return;
			case OrangeCharacter.SubStatus.SKILL1_2:
				this._refEntity.SetSpeed(0, 0);
				return;
			case OrangeCharacter.SubStatus.SKILL1_3:
				break;
			case OrangeCharacter.SubStatus.SKILL1_4:
				this._refEntity.SetAnimateId((HumanBase.AnimateId)69U);
				break;
			default:
				return;
			}
			break;
		default:
			return;
		}
	}

	public void AnimationEndCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
		if (mainStatus != OrangeCharacter.MainStatus.TELEPORT_IN) {
			if (mainStatus != OrangeCharacter.MainStatus.SKILL) {
				return;
			}
			if (subStatus - OrangeCharacter.SubStatus.SKILL0 <= 1) {
				this.SkillEndChnageToIdle(false);
				return;
			}
			switch (subStatus) {
			case OrangeCharacter.SubStatus.SKILL1:
				if (this._refEntity.IsLocalPlayer) {
					this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_1);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL1_1:
			case OrangeCharacter.SubStatus.SKILL1_2:
			case OrangeCharacter.SubStatus.SKILL1_3:
				break;
			case OrangeCharacter.SubStatus.SKILL1_4:
				this.SkillEndChnageToIdle(false);
				break;
			default:
				return;
			}
		}
		else if (subStatus == OrangeCharacter.SubStatus.TELEPORT_POSE) {
			this.ToggleWeapon(0);
			return;
		}
	}

	public override void CreateSkillBullet(WeaponStruct wsSkill) {
		this._refEntity.FreshBullet = true;
		OrangeCharacter.MainStatus curMainStatus = this._refEntity.CurMainStatus;
		if (curMainStatus == OrangeCharacter.MainStatus.SKILL) {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			if (curSubStatus - OrangeCharacter.SubStatus.SKILL0 <= 1) {
				Vector3 shootPosition = this._refEntity.ModelTransform.position + Vector3.right * (float)this._refEntity.direction;
				this._refEntity.PushBulletDetail(wsSkill.FastBulletDatas[0], wsSkill.weaponStatus, shootPosition, wsSkill.SkillLV, default(Il2CppSystem.Nullable_Unboxed<UnityEngine.Vector3>), true);
				this._refEntity.CheckUsePassiveSkill(0, wsSkill.weaponStatus, wsSkill.ShootTransform[0]);
				OrangeBattleUtility.UpdateSkillCD(wsSkill);
				return;
			}
			if (curSubStatus != OrangeCharacter.SubStatus.SKILL1) {
				return;
			}
			this._refEntity.PushBulletDetail(wsSkill.FastBulletDatas[0], wsSkill.weaponStatus, this._refEntity.AimTransform, wsSkill.SkillLV, new Il2CppSystem.Nullable_Unboxed<UnityEngine.Vector3>(this._vSkillVelocity), true, default(Il2CppSystem.Nullable_Unboxed<int>), (CallbackObj)new Action<Il2CppSystem.Object>(this.Skill1MoveHitCB));
			this._refEntity.CheckUsePassiveSkill(1, wsSkill.weaponStatus, wsSkill.ShootTransform[0]);
			OrangeBattleUtility.UpdateSkillCD(wsSkill);
		}
	}

	public void CreateShungokusatsuBullet(WeaponStruct wsSkill) {
		if (this._pShungokusatsuBullet) {
			this._pShungokusatsuBullet.SetHitTarget(this._tfShungokusatsuTarget);
			BulletBase pShungokusatsuBullet = this._pShungokusatsuBullet;
			SKILL_TABLE tSkill1_LinkSkill = this._tSkill1_LinkSkill;
			string sPlayerName = this._refEntity.sPlayerName;
			int nowRecordNO = this._refEntity.GetNowRecordNO();
			OrangeCharacter refEntity = this._refEntity;
			int nBulletRecordID = refEntity.nBulletRecordID;
			refEntity.nBulletRecordID = nBulletRecordID + 1;
			pShungokusatsuBullet.UpdateBulletData(tSkill1_LinkSkill, sPlayerName, nowRecordNO, nBulletRecordID, this._refEntity.direction);
			this._pShungokusatsuBullet.SetBulletAtk(wsSkill.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
			this._pShungokusatsuBullet.BulletLevel = wsSkill.SkillLV;
			if (this._tfShungokusatsuTarget) {
				this._pShungokusatsuBullet.Active(this._tfShungokusatsuTarget.transform.position, this._refEntity.ShootDirection, this._refEntity.TargetMask, null);
			}
			else {
				Vector3 pPos = this._refEntity.AimPosition + Vector3.right * (float)this._refEntity.direction;
				this._pShungokusatsuBullet.Active(pPos, this._refEntity.ShootDirection, this._refEntity.TargetMask, null);
			}
		}
		this._tfShungokusatsuTarget = null;
	}

	public override void CallPet(int petID, bool isHurt, int nSetNumID = -1, Il2CppSystem.Nullable_Unboxed<Vector3> vSetPos = default(Il2CppSystem.Nullable_Unboxed<Vector3>)) {
	}

	public void CheckSkillLockDirection() {
		OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
		if (curSubStatus - OrangeCharacter.SubStatus.SKILL1 > 4) {
			//this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
			this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
		}
	}

	public void TeleportInExtraEffect() {
		Vector3 p_worldPos = this._refEntity.ModelTransform.position + new Vector3(1.09f, 0f, 0f) * (float)this._refEntity.direction;
		FxManager_.Play(this.GetTeleportInExtraEffect(), p_worldPos, (this._refEntity.direction == 1) ? OrangeBattleUtility.QuaternionNormal : OrangeBattleUtility.QuaternionReverse);
	}

	public override string GetTeleportInExtraEffect() {
		return "fxdemo_gouki_000";
	}

	private void UpdateSkill() {
		OrangeCharacter.MainStatus curMainStatus = this._refEntity.CurMainStatus;
		if (curMainStatus == OrangeCharacter.MainStatus.SKILL) {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			if (curSubStatus - OrangeCharacter.SubStatus.SKILL0 > 1) {
				switch (curSubStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
					if (this._refEntity.CurrentFrame > 1f) {
						if (this._refEntity.IsLocalPlayer) {
							this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_1);
							return;
						}
						if (this._bSyncSkillTargetCompleted) {
							if (this._pSyncTarget != null) {
								this._refEntity.PlayerAutoAimSystem.AutoAimTarget = this._pSyncTarget;
								this.TurnToAimTarget();
								this._vSkillVelocity = (this._pSyncTarget.AimPosition - this._refEntity.AimPosition).normalized * ((float)OrangeCharacter.DashSpeed * 4f);
								this.UpdateSkill1Direction(this._vSkillVelocity.x);
							}
							else {
								this._vSkillVelocity = this._vSyncDirection.normalized * ((float)OrangeCharacter.DashSpeed * 4f);
								this.UpdateSkill1Direction(this._vSkillVelocity.x);
							}
							this._bSyncSkillTargetCompleted = false;
							this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_1);
							return;
						}
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_1:
					if (this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
						this._otSkill1Timer.TimerStart();
						this._refEntity.BulletCollider.BackToPool();
						this._refEntity.BulletCollider.HitCallback = null;
						this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_2);
						Vector3 p_worldPos = this._refEntity.AimPosition + new Vector3(1.5f, 0f, 0f) * (float)this._refEntity.direction;
						this._pShungokusatsuFx = FxManager_.PlayReturn<CH100_ShungokusatsuHitFx>("fxhit_shungokusatsu_000", p_worldPos, Quaternion.identity);
						bool visible = this.IsHitPlayer(-1);
						this._pShungokusatsuFx.ActivePlayBlackBG(visible);
						if (this.m_fxuse_skill != null && this.m_fxuse_skill.isPlaying) {
							this.m_fxuse_skill.Stop();
							return;
						}
					}
					else if (Vector2.Distance(this._vSkillStartPosition, this._refEntity.AimPosition) > this._refEntity.PlayerSkills[1].BulletData.f_DISTANCE || this._refEntity.PlayerSkills[1].LastUseTimer.GetMillisecond() > 350L) {
						this._refEntity.SetSpeed(0, 0);
						this._refEntity.BulletCollider.BackToPool();
						this.SkipSkill1Animation();
						if (this.m_fxuse_skill != null && this.m_fxuse_skill.isPlaying) {
							this.m_fxuse_skill.Stop();
							return;
						}
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_2:
					if (this.bInSkill) {
						this.bInSkill = false;
						this.CreateShungokusatsuBullet(this._refEntity.PlayerSkills[1]);
						return;
					}
					if (this._refEntity.AnimateID != (HumanBase.AnimateId)68U && this._otSkill1Timer.GetMillisecond() > 100L) {
						this._refEntity.SetAnimateId((HumanBase.AnimateId)68U);
						return;
					}
					if (this._otSkill1Timer.GetMillisecond() > 800L) {
						base.PlaySkillSE("go_shungokusatsu03");
						this._fxSkill1Shinzin = FxManager_.PlayReturn<FxBase>("fxuse_shungokusatsu_003", this._refEntity.ExtraTransforms[4], Quaternion.identity);
						this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_3);
						FxManager_.Play("fxuse_shungokusatsu_004", this._refEntity.ModelTransform.position, Quaternion.identity);
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_3:
					if (this._otSkill1Timer.GetMillisecond() > 1800L) {
						this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_4);
						return;
					}
					if (this.CheckCancelAnimate(1) && this._otSkill1Timer.GetMillisecond() > 1400L) {
						if (this._fxSkill1Shinzin) {
							this._fxSkill1Shinzin.StopEmittingBackToPool(0f);
						}
						this.SkipSkill1Animation();
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_4:
					if (this._refEntity.CurrentFrame > 1f) {
						this.SkillEndChnageToIdle(false);
						return;
					}
					if (this.CheckCancelAnimate(1) && this._refEntity.CurrentFrame > 0.5f) {
						this.SkipSkill1Animation();
					}
					break;
				default:
					return;
				}
			}
			else {
				if (this._refEntity.CurrentFrame > 1f) {
					this.SkillEndChnageToIdle(false);
					return;
				}
				if (this.bInSkillFx && this._refEntity.CurrentFrame > 0.08f) {
					this.bInSkillFx = false;
					FxManager_.Play("fxuse_sekia_000", this._refEntity.ExtraTransforms[2], Quaternion.identity);
					return;
				}
				if (this.bInSkill && this._refEntity.CurrentFrame > 0.55f) {
					this.bInSkill = false;
					this.CreateSkillBullet(this._refEntity.PlayerSkills[0]);
					return;
				}
				if (this.CheckCancelAnimate(0) && this._refEntity.CurrentFrame > 0.7f) {
					this.SkipSkill0Animation();
					return;
				}
			}
		}
	}

	private bool IsHitPlayer(int buffId) {
		foreach (PerBuff perBuff in this._refEntity.selfBuffManager.listBuffs) {
			if (perBuff.nBuffID == buffId && !string.IsNullOrEmpty(perBuff.sPlayerID)) {
				return true;
			}
		}
		return false;
	}

	private void TurnToAimTarget() {
		/*
		Il2CppSystem.Nullable_Unboxed<Vector3> vector = this._refEntity.CalibrateAimDirection(this._refEntity.AimPosition);
		if (vector.has_value) {
			Vector3 v = vector.Value;
			float x = v.x;
			int num = Math.Sign(x);
			if ((int)this._refEntity._characterDirection != num && Mathf.Abs(x) > 0.05f) {
				this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
				this._refEntity.ShootDirection = v;
			}
		}
		*/
		IAimTarget target;
		OrangeCharacter _ref = this._refEntity;

		if (_ref._playerAutoAimSystem == null || _ref._playerAutoAimSystem.AutoAimTarget == null) { return; }
		target = _ref._playerAutoAimSystem.AutoAimTarget;

		Vector3 v = (target.AimPosition - _ref.AimPosition).normalized;
		float x = v.x;
		int num = Math.Sign(x);
		if ((int)_ref._characterDirection != num && Mathf.Abs(x) > 0.05f) {
			_ref._characterDirection = (CharacterDirection)(-(int)_ref._characterDirection);
			_ref.ShootDirection = v;
		}
	}

	private void UseSkill0(int skillId) {
		this.bInSkill = true;
		this.bInSkillFx = true;
		this._refEntity.CurrentActiveSkill = skillId;
		this._refEntity.SkillEnd = false;
		this._refEntity.PlayerStopDashing();
		this._refEntity.SetSpeed(0, 0);
		this._refEntity.IsShoot = 1;
		this.TurnToAimTarget();
		this.ToggleWeapon(1);
		if (this._refEntity.Controller.Collisions.below) {
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0);
			return;
		}
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0_1);
	}

	private void CancelSkill0() {
		this._refEntity.SkillEnd = true;
		this.SkipSkill0Animation();
	}

	private void SkipSkill0Animation() {
		this.SkillEndChnageToIdle(false);
	}

	private void UseSkill1(int skillId) {
		this.bInSkill = true;
		this._refEntity.CurrentActiveSkill = skillId;
		this._refEntity.SkillEnd = false;
		this._refEntity.PlayerStopDashing();
		this._refEntity.SetSpeed(0, 0);
		this._refEntity.IsShoot = 1;
		this._refEntity.IgnoreGravity = true;
		this.ToggleWeapon(2);
		if (this._refEntity.IsLocalPlayer) {
			if (this._refEntity.UseAutoAim) {
				this.TurnToAimTarget();
				IAimTarget autoAimTarget = this._refEntity.PlayerAutoAimSystem.AutoAimTarget;
				if (autoAimTarget != null) {
					this._vSkillVelocity = (autoAimTarget.AimPosition - this._refEntity.AimPosition).normalized * ((float)OrangeCharacter.DashSpeed * 4f);
					this.UpdateSkill1Direction(this._vSkillVelocity.x);
				}
				else {
					// this._vSkillVelocity = new Vector2((float)(this._refEntity._characterDirection * (CharacterDirection)((float)OrangeCharacter.DashSpeed * 4f)), 0f);
					this._vSkillVelocity = new Vector2((float)this._refEntity._characterDirection * ((float)OrangeCharacter.DashSpeed * 4f), 0f);
				}
			}
			else {
				this._vSkillVelocity = this._refEntity.ShootDirection * ((float)OrangeCharacter.DashSpeed * 4f);
				this.UpdateSkill1Direction(this._vSkillVelocity.x);
			}
		}
		FxManager_.Play("fxuse_shungokusatsu_000", this._refEntity.AimPosition, Quaternion.identity);
		FxManager_.Play("fxuse_shungokusatsu_001", this._refEntity.AimPosition, Quaternion.identity);
		this._vSkillStartPosition = this._refEntity.AimPosition;
		this._refEntity.PlayerSkills[1].LastUseTimer.TimerStart();
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1);
	}

	private void UpdateSkill1Direction(float dirX) {
		if (Mathf.Abs(dirX) > 0.05f) {
			int num = Math.Sign(dirX);
			if (num != this._refEntity.direction) {
				this._refEntity.direction = num;
			}
		}
	}

	private void CancelSkill1() {
		if (!this._refEntity.BulletCollider.bIsEnd) {
			this._refEntity.BulletCollider.BackToPool();
			this._refEntity.BulletCollider.HitCallback = null;
		}
		if (!this._pShungokusatsuBullet.bIsEnd) {
			this._pShungokusatsuBullet.BackToPool();
			if (this._pShungokusatsuFx) {
				this._pShungokusatsuFx.BackToPool();
				this._pShungokusatsuFx = null;
			}
		}
		this._refEntity.SkillEnd = true;
		this.SkipSkill1Animation();
	}

	private void SkipSkill1Animation() {
		this.SkillEndChnageToIdle(false);
	}

	private void Skill1MoveHitCB(Il2CppSystem.Object obj) {
		if (!this._refEntity.IsLocalPlayer && !this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
			return;
		}
		if (this._refEntity.UsingVehicle) {
			return;
		}
		Collider2D collider2D = new Collider2D(obj.Unbox<IntPtr>());
		if (collider2D == null) {
			return;
		}
		Transform transform = collider2D.transform;
		StageObjParam stageObjParam = transform.GetComponent<StageObjParam>();
		if (stageObjParam == null) {
			PlayerCollider component = transform.GetComponent<PlayerCollider>();
			if (component != null && component.IsDmgReduceShield()) {
				stageObjParam = component.GetDmgReduceOwner();
			}
		}
		if (stageObjParam == null || stageObjParam.tLinkSOB == null) {
			return;
		}
		OrangeCharacter orangeCharacter = stageObjParam.tLinkSOB as OrangeCharacter;
		EnemyControllerBase exists = stageObjParam.tLinkSOB as EnemyControllerBase;
		if (orangeCharacter || exists) {
			this._otSkill1Timer.TimerStart();
			this._tfShungokusatsuTarget = transform;
			this._refEntity.BulletCollider.BackToPool();
			this._refEntity.BulletCollider.HitCallback = null;
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_2);
			if (this._refEntity.IsLocalPlayer) {
				if (orangeCharacter) {
					this._refEntity.selfBuffManager.AddBuff(-1, 0, 0, 0, false, orangeCharacter.sPlayerID, 0);
				}
				else {
					this._refEntity.selfBuffManager.AddBuff(-1, 0, 0, 0, false, "", 0);
				}
			}
			Vector3 p_worldPos = this._refEntity.AimPosition + new Vector3(1.5f, 0f, 0f) * (float)this._refEntity.direction;
			base.PlaySkillSE("go_shungokusatsu02");
			this._pShungokusatsuFx = FxManager_.PlayReturn<CH100_ShungokusatsuHitFx>("fxhit_shungokusatsu_000", p_worldPos, Quaternion.identity);
			if (this._refEntity.IsLocalPlayer || orangeCharacter) {
				this._pShungokusatsuFx.ActivePlayBlackBG(true);
				return;
			}
			this._pShungokusatsuFx.ActivePlayBlackBG(false);
		}
	}

	private bool CheckCancelAnimate(int skilliD) {
		if (skilliD == 0) {
			if (ManagedSingleton<InputStorage>.Instance.IsAnyHeld(this._refEntity.UserID) && !ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.SKILL0)) {
				return true;
			}
		}
		else if (skilliD == 1 && ManagedSingleton<InputStorage>.Instance.IsAnyHeld(this._refEntity.UserID) && !ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.SKILL1)) {
			return true;
		}
		return false;
	}

	private void SkillEndChnageToIdle(bool isCrouch = false) {
		this._refEntity.SkillEnd = true;
		this._refEntity.Dashing = false;
		this._refEntity.IgnoreGravity = false;
		this._refEntity.GravityMultiplier = new VInt(1f);
		this._refEntity.Animator._animator.speed = 1f;
		this.bInSkill = false;
		this.bInSkillFx = false;
		this._tfShungokusatsuTarget = null;
		if (this._refEntity.IsLocalPlayer && this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
			this._refEntity.selfBuffManager.RemoveBuffByCONDITIONID(-1, true);
		}
		this.ToggleWeapon(0);
		if (isCrouch) {
			if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.DOWN)) {
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.CROUCH, OrangeCharacter.SubStatus.WIN_POSE);
				return;
			}
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.CROUCH_UP);
			return;
		}
		else {
			if (this._refEntity.Controller.Collisions.below) {
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
				return;
			}
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
			return;
		}
	}

	private void ToggleWeapon(int style) {
		switch (style) {
		case -3:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				return;
			}
			return;
		case -2:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				return;
			}
			return;
		case -1:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				return;
			}
			return;
		case 1:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				return;
			}
			return;
		case 2:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				return;
			}
			return;
		}
		if (this._refEntity.CheckCurrentWeaponIndex()) {
			this._refEntity.EnableCurrentWeapon();
		}
	}

	protected bool bInSkill;

	protected bool bInSkillFx;

	protected SKILL_TABLE _tSkill1_LinkSkill;

	protected Vector3 _vSkillStartPosition;

	protected Vector2 _vSkillVelocity;

	protected OrangeTimer _otSkill1Timer;

	protected CH100_ShungokusatsuBullet _pShungokusatsuBullet;

	protected CH100_ShungokusatsuHitFx _pShungokusatsuFx;

	protected Transform _tfShungokusatsuTarget;

	protected FxBase _fxSkill1Shinzin;

	private ParticleSystem m_fxuse_skill;

	protected bool _bSyncSkillTargetCompleted;

	protected IAimTarget _pSyncTarget;

	protected Vector3 _vSyncDirection;

	private OrangeConsoleCharacter _refPlayer;
}
