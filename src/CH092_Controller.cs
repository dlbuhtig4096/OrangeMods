using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH092_Controller : CharacterControlBase {

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch092_skill_01_start",
				"ch092_skill_01_end",
				"ch092_skill_02_step1_loop",
				"ch092_skill_02_step1_end",
				"ch092_skill_02_step2_start",
				"ch092_skill_02_step2_end"
			}
		);
	}

	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitLinkSkill();
		this.InitExtraMeshData();
	}

	private void InitExtraMeshData() {
		Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		this._refEntity.ExtraTransforms = new Transform[2];
		this._refEntity.ExtraTransforms[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "L WeaponPoint", true);
		this._refEntity.ExtraTransforms[1] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "R WeaponPoint", true);
		Transform transform = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "SaberMesh_m", true);
		this._tfWeaponMesh = transform.GetComponent<SkinnedMeshRenderer>();
		this._tfWeaponMesh.enabled = true;
		Transform transform2 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "RopeMesh_e", true);
		this._tfRopeMesh = transform2.GetComponent<SkinnedMeshRenderer>();
		this._tfRopeMesh.enabled = false;
		Transform transform3 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxuse_skill", true);
		if (transform3 != null) {
			this.m_fxuse_skill = transform3.GetComponent<ParticleSystem>();
		}
		this._refEntity.CDSkill(1);
		this._otSkill0Timer = OrangeTimerManager.GetTimer(TimerMode.FRAME);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_soaringkick_000", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_soaringkick_001", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_soaringkick_002", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxhit_spiritslash_000", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_spiritslash_000", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_spiritslash_001", 5, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx("fxuse_spiritslash_002", 5, null);
	}

	public override void ExtraVariableInit() {
		if (this.m_fxuse_skill != null && this.m_fxuse_skill.isPlaying) {
			this.m_fxuse_skill.Stop();
		}
	}

	private void InitLinkSkill() {
		if (this._tSkill0_LinkSkill == null && this._refEntity.PlayerSkills[0].BulletData.n_LINK_SKILL != 0) {
			WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
			if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(weaponStruct.BulletData.n_LINK_SKILL, out this._tSkill0_LinkSkill)) {
				this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this._tSkill0_LinkSkill);
				MonoBehaviourSingleton<AssetsBundleManager>.Instance.GetAssetAndAsyncLoad<GameObject>("prefab/bullet/" + this._tSkill0_LinkSkill.s_MODEL, this._tSkill0_LinkSkill.s_MODEL, (AssetsBundleManager.OnAsyncLoadAssetComplete<GameObject>)delegate(GameObject obj) {
					BulletBase component = obj.GetComponent<BulletBase>();
					MonoBehaviourSingleton<PoolManager>.Instance.CreatePoolBaseLocal<BulletBase>(UnityEngine.Object.Instantiate<BulletBase>(component), this._tSkill0_LinkSkill.s_MODEL, 5);
				}, 0 /*AssetsBundleManager.AssetKeepMode.KEEP_IN_SCENE*/);
			}
		}
		if (this._tSkill1_LinkSkill == null && this._refEntity.PlayerSkills[1].BulletData.n_LINK_SKILL != 0) {
			WeaponStruct weaponStruct2 = this._refEntity.PlayerSkills[1];
			if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(weaponStruct2.BulletData.n_LINK_SKILL, out this._tSkill1_LinkSkill)) {
				this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this._tSkill1_LinkSkill);
				GameObject gameObject = new GameObject();
				CollideBullet go = gameObject.AddComponent<CollideBullet>();
				gameObject.name = this._tSkill1_LinkSkill.s_MODEL;
				gameObject.layer = ManagedSingleton<OrangeLayerManager>.Instance.BulletLayer;
				MonoBehaviourSingleton<PoolManager>.Instance.CreatePoolBaseLocal<CollideBullet>(go, this._tSkill1_LinkSkill.s_MODEL, 5);
			}
		}
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
		this._refEntity.AnimationEndCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.AnimationEndCharacterDepend);
		this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
		this._refEntity.CheckSkillLockDirectionEvt = (Callback)new Action(this.CheckSkillLockDirection);
		this._refEntity.StageTeleportOutCharacterDependEvt = (Callback)new Action(this.StageTeleportOutCharacterDepend);
		this._refEntity.GuardCalculateEvt = new Func<HurtPassParam, bool>(this.GuardCalculate);
		this._refEntity.GuardHurtEvt = (Callback<HurtPassParam>)new Action<HurtPassParam>(this.GuardHurt);
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
		this.UpdateSkill();
	}

	public override void PlayerPressSkillCharacterCall(int id) {
		if (id != 0) {
			if (id != 1) {
				return;
			}
			if (this._refEntity.CurrentActiveSkill != id) {
				if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
					return;
				}
				this.UseSkill1(id);
			}
		}
		else if (this._refEntity.CurrentActiveSkill != id) {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			this.UseSkill0(id);
			return;
		}
	}

	public override void PlayerReleaseSkillCharacterCall(int id) {
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
				this.ToggleWeapon(-2);
				return;
			}
			break;
		case OrangeCharacter.MainStatus.SLASH:
			break;
		case OrangeCharacter.MainStatus.SKILL:
			switch (subStatus) {
			case OrangeCharacter.SubStatus.SKILL0:
				this._refEntity.Animator._animator.speed = 4f;
				this._refEntity.SetAnimateId(HumanBase.AnimateId.ANI_SKILL_START);
				return;
			case OrangeCharacter.SubStatus.SKILL0_1:
				this.CreateSkillBullet(this._refEntity.PlayerSkills[0]);
				return;
			case OrangeCharacter.SubStatus.SKILL0_2:
				break;
			case OrangeCharacter.SubStatus.SKILL0_3:
				this._refEntity.SetAnimateId((HumanBase.AnimateId)66U);
				return;
			default:
				switch (subStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
					this.CreateSkillBullet(this._refEntity.PlayerSkills[1]);
					if (this.m_fxuse_skill != null && !this.m_fxuse_skill.isPlaying) {
						this.m_fxuse_skill.Play();
					}
					this._refEntity.SetAnimateId((HumanBase.AnimateId)67U);
					return;
				case OrangeCharacter.SubStatus.SKILL1_1:
					this._refEntity.Animator._animator.speed = 2f;
					this._refEntity.SetSpeed(0, 0);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)68U);
					return;
				case OrangeCharacter.SubStatus.SKILL1_2:
					this._refEntity.Animator._animator.speed = 2f;
					this._refEntity.IgnoreGravity = false;
					this._refEntity.SetSpeed(0, OrangeCharacter.JumpSpeed);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)69U);
					return;
				case OrangeCharacter.SubStatus.SKILL1_3:
					base.PlaySkillSE("mh1_hisyo02");
					this._refEntity.Animator._animator.speed = 1f;
					this._refEntity.GravityMultiplier = new VInt(4f);
					this._refEntity.SetAnimateId((HumanBase.AnimateId)70U);
					break;
				default:
					return;
				}
				break;
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
			switch (subStatus) {
			case OrangeCharacter.SubStatus.SKILL0:
			case OrangeCharacter.SubStatus.SKILL0_1:
			case OrangeCharacter.SubStatus.SKILL0_2:
				break;
			case OrangeCharacter.SubStatus.SKILL0_3:
				this.SkillEndChnageToIdle(false);
				return;
			default:
				switch (subStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
				case OrangeCharacter.SubStatus.SKILL1_2:
					break;
				case OrangeCharacter.SubStatus.SKILL1_1:
					this._tfRopeMesh.enabled = false;
					this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_2);
					return;
				case OrangeCharacter.SubStatus.SKILL1_3:
					this.SkillEndChnageToIdle(false);
					break;
				default:
					return;
				}
				break;
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
			if (curSubStatus == OrangeCharacter.SubStatus.SKILL0_1) {
				BulletBase bulletCollider = this._refEntity.BulletCollider;
				SKILL_TABLE pData = wsSkill.FastBulletDatas[0];
				string sPlayerName = this._refEntity.sPlayerName;
				int nowRecordNO = this._refEntity.GetNowRecordNO();
				OrangeCharacter refEntity = this._refEntity;
				int nBulletRecordID = refEntity.nBulletRecordID;
				refEntity.nBulletRecordID = nBulletRecordID + 1;
				bulletCollider.UpdateBulletData(pData, sPlayerName, nowRecordNO, nBulletRecordID, this._refEntity.direction);
				this._refEntity.BulletCollider.SetBulletAtk(wsSkill.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
				this._refEntity.BulletCollider.BulletLevel = wsSkill.SkillLV;
				this._refEntity.BulletCollider.Active(this._refEntity.TargetMask);
				this._refEntity.BulletCollider.HitCallback = null;
				this._refEntity.CheckUsePassiveSkill(0, wsSkill.weaponStatus, this._refEntity.ExtraTransforms[0]);
				OrangeBattleUtility.UpdateSkillCD(wsSkill);
				return;
			}
			if (curSubStatus == OrangeCharacter.SubStatus.SKILL1) {
				BulletBase bulletCollider2 = this._refEntity.BulletCollider;
				SKILL_TABLE pData2 = wsSkill.FastBulletDatas[0];
				string sPlayerName2 = this._refEntity.sPlayerName;
				int nowRecordNO2 = this._refEntity.GetNowRecordNO();
				OrangeCharacter refEntity2 = this._refEntity;
				int nBulletRecordID = refEntity2.nBulletRecordID;
				refEntity2.nBulletRecordID = nBulletRecordID + 1;
				bulletCollider2.UpdateBulletData(pData2, sPlayerName2, nowRecordNO2, nBulletRecordID, this._refEntity.direction);
				this._refEntity.BulletCollider.SetBulletAtk(wsSkill.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
				this._refEntity.BulletCollider.BulletLevel = wsSkill.SkillLV;
				this._refEntity.BulletCollider.Active(this._refEntity.TargetMask);
				this._refEntity.BulletCollider.HitCallback = (CallbackObj)new Action<Il2CppSystem.Object>(this.Skill1FlyKickHitCB);
				this._refEntity.CheckUsePassiveSkill(1, wsSkill.weaponStatus, this._refEntity.ExtraTransforms[0]);
				OrangeBattleUtility.UpdateSkillCD(wsSkill);
				return;
			}
			if (curSubStatus != OrangeCharacter.SubStatus.SKILL1_3) {
				return;
			}
			this._refEntity.PushBulletDetail(this._tSkill1_LinkSkill, wsSkill.weaponStatus, this._refEntity.ModelTransform, wsSkill.SkillLV);
		}
	}

	public void CheckSkillLockDirection() {
		OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
		if (curSubStatus - OrangeCharacter.SubStatus.SKILL0 > 3 && curSubStatus - OrangeCharacter.SubStatus.SKILL1 > 3) {
			this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
			// this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
		}
	}

	public void StageTeleportOutCharacterDepend() {
		this._tfRopeMesh.enabled = false;
	}

	public override bool GuardCalculate(HurtPassParam tHurtPassParam) {
		return this._refEntity.Hp > 0 && (this._refEntity.CurMainStatus == OrangeCharacter.MainStatus.SKILL && (this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.SKILL0 || this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.SKILL0_1 || this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.SKILL0_2));
	}

	public void GuardHurt(HurtPassParam tHurtPassParam) {
		if (this._refEntity.IsLocalPlayer && this._refEntity.CurMainStatus == OrangeCharacter.MainStatus.SKILL && this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.SKILL0_1 && this._nSkill0_GuardCount == 0) {
			this._nSkill0_GuardCount++;
			this._refEntity.PushBulletDetail(this._tSkill0_LinkSkill, this._refEntity.PlayerSkills[0].weaponStatus, this._refEntity.AimTransform, this._refEntity.PlayerSkills[0].SkillLV);
		}
	}

	private void UpdateSkill() {
		OrangeCharacter.MainStatus curMainStatus = this._refEntity.CurMainStatus;
		if (curMainStatus == OrangeCharacter.MainStatus.SKILL) {
			OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
			switch (curSubStatus) {
			case OrangeCharacter.SubStatus.SKILL0:
				if (this.bPlaySkillFx && this._refEntity.CurrentFrame > 0.28f) {
					this.bPlaySkillFx = false;
					Vector3 p_worldPos = this._refEntity.ModelTransform.position + new Vector3(0f, 0.67f, 0f);
					FxManager_.Play("fxuse_spiritslash_000", p_worldPos, OrangeCharacter.NormalQuaternion);
				}
				if (this.bPlaySkillFx2 && this._refEntity.CurrentFrame > 0.5f) {
					this.bPlaySkillFx2 = false;
					Vector3 p_worldPos2 = this._refEntity.ModelTransform.position + new Vector3(0f, 0.67f, 0f);
					FxManager_.Play("fxuse_spiritslash_001", p_worldPos2, OrangeCharacter.NormalQuaternion);
					return;
				}
				if (this._refEntity.CurrentFrame > 0.55f) {
					this._vSkillVelocity = Vector3.right * (float)OrangeCharacter.DashSpeed * 4f * (float)this._refEntity.direction;
					this._vSkillStartPosition = this._refEntity.AimPosition;
					FxManager_.Play("fxuse_spiritslash_002", this._refEntity.ModelTransform, OrangeCharacter.NormalQuaternion);
					this._refEntity.Animator._animator.speed = 1f;
					this._refEntity.SetSpeed((int)this._vSkillVelocity.x, (int)this._vSkillVelocity.y);
					this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0_1);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_1:
				if (Vector2.Distance(this._vSkillStartPosition, this._refEntity.AimPosition) > this._refEntity.PlayerSkills[0].BulletData.f_DISTANCE || this._refEntity.PlayerSkills[0].LastUseTimer.GetMillisecond() > 500L) {
					this._otSkill0Timer.TimerStart();
					this._refEntity.BulletCollider.BackToPool();
					this._refEntity.SetSpeed(OrangeCharacter.WalkSpeed * this._refEntity.direction, 0);
					this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0_2);
					return;
				}
				break;
			case OrangeCharacter.SubStatus.SKILL0_2:
				if (this.CheckCancelAnimate(0)) {
					this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
					// this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
					this._refEntity.IsShoot = 0;
					this.SkipSkill0Animation();
					return;
				}
				if (this._otSkill0Timer.GetMillisecond() < 500L) {
					int x = Mathf.RoundToInt((float)((long)OrangeCharacter.WalkSpeed - (long)(OrangeCharacter.WalkSpeed / 500) * this._otSkill0Timer.GetMillisecond())) * this._refEntity.direction;
					this._refEntity.SetSpeed(x, 0);
					return;
				}
				this._refEntity.IgnoreGravity = false;
				this._refEntity.SetSpeed(0, 0);
				this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
				// this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
				this._refEntity.IsShoot = 0;
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0_3);
				return;
			case OrangeCharacter.SubStatus.SKILL0_3:
				if (this._refEntity.CurrentFrame > 1f) {
					this.SkillEndChnageToIdle(false);
					return;
				}
				if (this.CheckCancelAnimate(0)) {
					this.SkipSkill0Animation();
					return;
				}
				break;
			default:
				switch (curSubStatus) {
				case OrangeCharacter.SubStatus.SKILL1:
					if (Vector2.Distance(this._vSkillStartPosition, this._refEntity.AimPosition) > this._refEntity.PlayerSkills[1].BulletData.f_DISTANCE || this._refEntity.PlayerSkills[1].LastUseTimer.GetMillisecond() > 500L) {
						this._tfRopeMesh.enabled = false;
						if (!this._refEntity.Controller.Collisions.below) {
							this._refEntity.SetSpeed(this._refEntity.direction * OrangeCharacter.WalkSpeed, 0);
						}
						else {
							this._refEntity.SetSpeed(0, 0);
						}
						this._refEntity.BulletCollider.BackToPool();
						this.SkipSkill1Animation();
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_1:
					if (this._refEntity.CurrentFrame > 1f) {
						this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_2);
						return;
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_2:
					if (this._refEntity.CurrentFrame > 1f) {
						this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_3);
						return;
					}
					if (this._refEntity.CurrentFrame > 0.3f) {
						if ((float)this._refEntity.Velocity.y > (float)OrangeCharacter.JumpSpeed * 0.2f) {
							this._refEntity.Animator._animator.speed = 0.8f;
							this._refEntity.SetSpeed(0, (int)((float)OrangeCharacter.JumpSpeed * 0.2f));
							return;
						}
						if ((float)this._refEntity.Velocity.y < 0f) {
							this._refEntity.SetSpeed(0, 0);
							return;
						}
					}
					break;
				case OrangeCharacter.SubStatus.SKILL1_3:
					if (this._refEntity.CurrentFrame > 1f) {
						this.SkillEndChnageToIdle(false);
						return;
					}
					if (this.bInSkill && this._refEntity.CurrentFrame > 0.2f) {
						this.bInSkill = false;
						Vector3 p_worldPos3 = this._refEntity.ModelTransform.position + new Vector3((float)this._refEntity.direction, -1f, 0f);
						FxManager_.Play("fxuse_soaringkick_000", p_worldPos3, OrangeCharacter.NormalQuaternion);
						p_worldPos3 = this._refEntity.ModelTransform.position + new Vector3((float)this._refEntity.direction, 0f, 0f);
						FxManager_.Play("fxuse_soaringkick_001", p_worldPos3, OrangeCharacter.NormalQuaternion);
						this.CreateSkillBullet(this._refEntity.PlayerSkills[1]);
						return;
					}
					if (this.CheckCancelAnimate(1) && this._refEntity.CurrentFrame > 0.3f) {
						this.SkipSkill1Animation();
					}
					break;
				default:
					return;
				}
				break;
			}
		}
	}

	private void TurnToAimTarget() {
		Il2CppSystem.Nullable_Unboxed<Vector3>? vector = this._refEntity.CalibrateAimDirection(this._refEntity.AimPosition);
		if (vector != null) {
			Vector3 v = vector.Value.Value;
			float x = v.x;
			int num = Math.Sign(x);
			/*
			if (this._refEntity._characterDirection != (CharacterDirection)num && Mathf.Abs(vector.Value.x) > 0.05f) {
				this._refEntity._characterDirection = this._refEntity._characterDirection * CharacterDirection.LEFT;
				this._refEntity.ShootDirection = vector.Value;
			}
			*/
			if ((int)this._refEntity._characterDirection != num && Mathf.Abs(x) > 0.05f) {
				this._refEntity._characterDirection = (CharacterDirection)(-(int)this._refEntity._characterDirection);
				this._refEntity.ShootDirection = v;
			}
		}
	}

	private void UseSkill0(int skillId) {
		this.bInSkill = true;
		this._refEntity.CurrentActiveSkill = skillId;
		this._refEntity.SkillEnd = false;
		this._refEntity.SetSpeed(0, 0);
		this._refEntity.IsShoot = 1;
		this._refEntity.IgnoreGravity = true;
		this.ToggleWeapon(1);
		this._nSkill0_GuardCount = 0;
		this.bPlaySkillFx = true;
		this.bPlaySkillFx2 = true;
		base.PlayVoiceSE("v_mh1_skill01");
		base.PlaySkillSE("mh1_iai01");
		this._refEntity.PlayerStopDashing();
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL0);
	}

	private void CancelSkill0() {
		this._refEntity.SkillEnd = true;
		if (!this._refEntity.BulletCollider.bIsEnd) {
			this._refEntity.BulletCollider.BackToPool();
		}
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
		this.ToggleWeapon(2);
		this.TurnToAimTarget();
		this._refEntity.IgnoreGravity = true;
		base.PlayVoiceSE("v_mh1_skill02");
		this._pSkillTarget = this._refEntity.PlayerAutoAimSystem.AutoAimTarget;
		if (this._pSkillTarget != null) {
			this._vSkillVelocity = (this._pSkillTarget.AimPosition - this._refEntity.AimPosition).normalized * ((float)OrangeCharacter.DashSpeed * 4f);
			int num = Math.Sign(this._vSkillVelocity.x);
			if (num != this._refEntity.direction && num != 0) {
				this._refEntity.direction = Math.Sign(this._vSkillVelocity.x);
			}
		}
		else {
			// this._vSkillVelocity = new Vector2((float)(this._refEntity._characterDirection * (CharacterDirection)((float)OrangeCharacter.DashSpeed * 4f)), 0f);
			this._vSkillVelocity = new Vector2((float)((int)this._refEntity._characterDirection * ((float)OrangeCharacter.DashSpeed * 4f)), 0f);
		}
		this._vSkillStartPosition = this._refEntity.AimPosition;
		this._refEntity.SetSpeed((int)this._vSkillVelocity.x, (int)this._vSkillVelocity.y);
		this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1);
	}

	private void CancelSkill1() {
		this._refEntity.SkillEnd = true;
		this.SkipSkill1Animation();
	}

	private void SkipSkill1Animation() {
		this.SkillEndChnageToIdle(false);
	}

	private void Skill1FlyKickHitCB(Il2CppSystem.Object obj) {
		if (this._refEntity.BulletCollider.HitTarget == null || this._refEntity.UsingVehicle) {
			return;
		}
		StageObjParam component = this._refEntity.BulletCollider.HitTarget.transform.GetComponent<StageObjParam>();
		if (component == null || component.tLinkSOB == null) {
			return;
		}
		UnityEngine.Object exists = component.tLinkSOB as OrangeCharacter;
		EnemyControllerBase exists2 = component.tLinkSOB as EnemyControllerBase;
		if (exists || exists2) {
			this._refEntity.BulletCollider.BackToPool();
			this._refEntity.BulletCollider.HitCallback = null;
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_1);
			Vector3 p_worldPos = this._refEntity.ModelTransform.position + new Vector3((float)this._refEntity.direction * 1.5f, 1f, 0f);
			FxManager_.Play("fxuse_soaringkick_002", p_worldPos, OrangeCharacter.NormalQuaternion);
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

	private void DebutOrClearStageToggleWeapon(bool bDebut) {
		this.ToggleWeapon(-1);
	}

	private void ToggleWeapon(int style) {
		switch (style) {
		case -3:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				this._refEntity.EnableHandMesh(true);
			}
			this._tfWeaponMesh.enabled = false;
			this._tfRopeMesh.enabled = true;
			return;
		case -2:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				this._refEntity.EnableHandMesh(true);
			}
			this._tfWeaponMesh.enabled = false;
			this._tfRopeMesh.enabled = false;
			return;
		case -1:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
				this._refEntity.EnableHandMesh(true);
			}
			this._tfWeaponMesh.enabled = true;
			this._tfRopeMesh.enabled = false;
			return;
		case 1:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			}
			this._tfWeaponMesh.enabled = true;
			this._tfRopeMesh.enabled = false;
			return;
		case 2:
			if (this._refEntity.CheckCurrentWeaponIndex()) {
				this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
			}
			this._tfWeaponMesh.enabled = true;
			this._tfRopeMesh.enabled = false;
			return;
		}
		if (this._refEntity.CheckCurrentWeaponIndex()) {
			this._refEntity.EnableCurrentWeapon();
		}
		this._tfWeaponMesh.enabled = false;
		this._tfRopeMesh.enabled = false;
	}

	protected bool bInSkill;

	protected bool bPlaySkillFx;

	protected bool bPlaySkillFx2;

	protected SkinnedMeshRenderer _tfWeaponMesh;

	protected SkinnedMeshRenderer _tfRopeMesh;

	protected SKILL_TABLE _tSkill0_LinkSkill;

	protected SKILL_TABLE _tSkill1_LinkSkill;

	protected Vector3 _vSkillStartPosition;

	protected Vector2 _vSkillVelocity;

	protected IAimTarget _pSkillTarget;

	private ParticleSystem m_fxuse_skill;

	protected int _nSkill0_GuardCount;

	protected OrangeTimer _otSkill0Timer;

	// [SerializeField]
	protected Vector3 _vSkill0FxPosition = new Vector3();
}
