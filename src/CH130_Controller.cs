using System;
using System.Collections;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH130_Controller : CharacterControlBase_ {

    public CH130_Controller() : base() {}
    
    public CH130_Controller(IntPtr p) : base(p) {}
    
    private void OnEnable() {
        MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.AddUpdate(new ILogicUpdate(this.Pointer));
    }

    private void OnDisable() {
        MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.RemoveUpdate(new ILogicUpdate(this.Pointer));
    }

    public override void Start() {
        base.Start();
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
        SKILL_TABLE skill_TABLE;
        if (ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(this.sklFrenzyId, out skill_TABLE)) {
            this.conditionId = skill_TABLE.n_CONDITION_ID;
            if (skill_TABLE.n_LINK_SKILL != 0 && ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(skill_TABLE.n_LINK_SKILL, out this.linkSklPassive)) {
                this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this.linkSklPassive);
                MonoBehaviourSingleton<PoolManager>.Instance.CreatePoolBase<ShieldBullet>("prefab/bullet/" + this.linkSklPassive.s_MODEL, this.linkSklPassive.s_MODEL, 3, (Callback)new Action(this.PreloadLinkComplete));
            }
        }
        ManagedSingleton<CharacterControlHelper>.Instance.PreloadLinkSkl<CollideBullet>(this._refEntity, 1, out this.linkSkl1);
        this._regularEffect = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_GoreICO_000", false).GetComponent<ParticleSystem>();
        this._regularEffect.Play(true);
        this._FrenzyEffect = new ParticleSystem[2];
        this._FrenzyEffect[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_barrage_000_L", false).GetComponent<ParticleSystem>();
        this._FrenzyEffect[1] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_barrage_000_R", false).GetComponent<ParticleSystem>();
        this._FrenzyEffect[0].Stop(true);
        this._FrenzyEffect[1].Stop(true);
        this._refPlayer = (this._refEntity as OrangeConsoleCharacter);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_0_00, 2, null);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_00, 2, null);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_2_00, 2, null);
    }

    private void PreloadLinkComplete() {
        this.isInit = true;
    }

    public override void OverrideDelegateEvent() {
        this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
        this._refEntity.TeleportOutCharacterDependEvt = (Callback)new Action(this.TeleportOutCharacterDepend);
        this._refEntity.StageTeleportOutCharacterDependEvt = (Callback)new Action(this.StageTeleportOutCharacterDepend);
        this._refEntity.StageTeleportInCharacterDependEvt = (Callback)new Action(this.StageTeleportInCharacterDepend);
        this._refEntity.PlayTeleportOutEffectEvt = (Callback)new Action(this.PlayTeleportOutEffect);
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
            this.CheckRushBuff();
        }
    }

    private void CheckFrenzyBuff() {
        if (this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(this.conditionId, 0)) {
            if (this._pShieldBullet == null) {
                this.CreateShieldBullet();
                this.PlayFrenzyFx();
                return;
            }
        }
        else if (this._pShieldBullet != null) {
            this._pShieldBullet.BackToPool();
            this._pShieldBullet = null;
            this.StopFrenzyFx();
        }
    }

    private void CheckRushBuff() {
        if (this._pRushCollideBullet != null && !this._refEntity.IsLocalPlayer && !this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
            this.RecoverRushCollideBullet(false);
        }
    }

    private void CreateShieldBullet() {
        WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
        this._pShieldBullet = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<ShieldBullet>(this.linkSklPassive.s_MODEL);
        BulletBase pShieldBullet = this._pShieldBullet;
        SKILL_TABLE pData = this.linkSklPassive;
        string sPlayerName = this._refEntity.sPlayerName;
        int nowRecordNO = this._refEntity.GetNowRecordNO();
        OrangeCharacter refEntity = this._refEntity;
        int nBulletRecordID = refEntity.nBulletRecordID;
        refEntity.nBulletRecordID = nBulletRecordID + 1;
        pShieldBullet.UpdateBulletData(pData, sPlayerName, nowRecordNO, nBulletRecordID, 1);
        this._pShieldBullet.SetBulletAtk(weaponStruct.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
        this._pShieldBullet.BulletLevel = 0;
        this._pShieldBullet.BindBuffId(this.conditionId, this._refEntity.IsLocalPlayer, false);
        this._pShieldBullet.Active(this._refEntity.ExtraTransforms[0], Quaternion.identity, this._refEntity.TargetMask, true, null);
    }

    private void PlayFrenzyFx() {
        this._FrenzyEffect[0].Play(true);
        this._FrenzyEffect[1].Play(true);
        FxManager_.Play(this.FX_2_00, this._refEntity.AimTransform.position, OrangeCharacter.NormalQuaternion);
        base.PlaySkillSE("ic2_black01_lp");
    }

    private void StopFrenzyFx() {
        this._FrenzyEffect[0].Stop(true);
        this._FrenzyEffect[1].Stop(true);
        base.PlaySkillSE("ic2_black01_stop");
    }

    public override void PlayerPressSkillCharacterCall(int id) {
    }

    public override void PlayerReleaseSkillCharacterCall(int id) {
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
            this._refEntity.CurrentActiveSkill = id;
            this._refEntity.SkillEnd = false;
            this._refEntity.PlayerStopDashing();
            this._refEntity.SetSpeed(0, 0);
            this._refEntity.IsShoot = 1;
            this._bSyncRushSkillCompleted = false;
            this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0f);
            if (this._refEntity.IsLocalPlayer) {
                if (this._refEntity.UseAutoAim && this._refEntity.PlayerAutoAimSystem.AutoAimTarget != null) {
                    ManagedSingleton<CharacterControlHelper>.Instance.TurnToAimTarget(this._refEntity);
                    this._pRushTarget = this._refEntity.PlayerAutoAimSystem.AutoAimTarget;
                    this._vSkillVelocity = (this._pRushTarget.AimPosition - this._refEntity.AimPosition).normalized * this.GetRushSpd();
                    this.UpdateSkill1Direction(this._vSkillVelocity.x);
                }
                else {
                    this._pRushTarget = null;
                    this._vSkillVelocity = this._refEntity.ShootDirection.normalized * this.GetRushSpd();
                    this.UpdateSkill1Direction(this._vSkillVelocity.x);
                }
                WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
                OrangeBattleUtility.UpdateSkillCD(weaponStruct);
                this._refEntity.CheckUsePassiveSkill(1, weaponStruct.BulletData, weaponStruct.weaponStatus, weaponStruct.ShootTransform[0]);
            }
            this.isSkillEventEnd = false;
            this._vSkillStartPosition = this._refEntity.AimPosition;
            this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1);
            this._refEntity.SetAnimateId((HumanBase.AnimateId)68U);
            this._refEntity.IgnoreGravity = true;
            return;
        }
        else {
            if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
                return;
            }
            this.isSkillEventEnd = false;
            this._refEntity.IsShoot = 1;
            this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
            ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_TRIGGER, this.SKL0_END, OrangeCharacter.SubStatus.SKILL0, out this.skillEventFrame, out this.endFrame);
            ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)67U, true);
            return;
        }
    }

    public override void CheckSkill() {
        if (this._refEntity.IsAnimateIDChanged()) {
            return;
        }
        this.UpdateVirtualButtonAnalog();
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
            if (curSubStatus != OrangeCharacter.SubStatus.SKILL0) {
                switch (curSubStatus) {
                case OrangeCharacter.SubStatus.SKILL1:
                    if (this._refEntity.CurrentFrame > 1f) {
                        bool flag = false;
                        if (this._refEntity.IsLocalPlayer) {
                            flag = true;
                        }
                        else if (this._bSyncRushSkillCompleted) {
                            this._bSyncRushSkillCompleted = false;
                            flag = true;
                        }
                        if (flag) {
                            WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
                            this._refEntity.PushBulletDetail(weaponStruct.BulletData, weaponStruct.weaponStatus, this._refEntity.AimTransform, weaponStruct.SkillLV, new Il2CppSystem.Nullable_Unboxed<UnityEngine.Vector3>(this._vSkillVelocity), true, default(Il2CppSystem.Nullable_Unboxed<int>), (CallbackObj)new Action<Il2CppSystem.Object>(this.RushSkillHitCB));
                            this._vSkillVelocity = this._vSkillVelocity.normalized * this.GetRushSpd();
                            this.UpdateSkill1Direction(this._vSkillVelocity.x);
                            this._refEntity.SetStatus(OrangeCharacter.MainStatus.SKILL, OrangeCharacter.SubStatus.SKILL1_1);
                            this._refEntity.SetAnimateId((HumanBase.AnimateId)69U);
                            this._refEntity.IgnoreGravity = true;
                            this._refEntity.SetSpeed((int)this._vSkillVelocity.x, (int)this._vSkillVelocity.y);
                            this.endFrame = GameLogicUpdateManager.GameFrame + this.SKL1_START;
                            FxManager_.Play(this.FX_1_00, this._refEntity.ModelTransform, Quaternion.identity);
                            return;
                        }
                    }
                    break;
                case OrangeCharacter.SubStatus.SKILL1_1: {
                    bool flag2 = false;
                    bool flag3 = false;
                    if (this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
                        flag2 = true;
                        flag3 = true;
                    }
                    else if (Vector2.Distance(this._vSkillStartPosition, this._refEntity.AimPosition) > this._refEntity.PlayerSkills[1].BulletData.f_DISTANCE || this._refEntity.PlayerSkills[1].LastUseTimer.GetMillisecond() > 350L) {
                        flag2 = true;
                        flag3 = false;
                        this._pRushTarget = null;
                    }
                    if (flag2) {
                        if (!this._refEntity.Controller.Collisions.below) {
                            this._refEntity.SetSpeed(this._refEntity.direction * OrangeCharacter.WalkSpeed, 0);
                        }
                        else {
                            this._refEntity.SetSpeed(0, 0);
                        }
                        this.RecoverRushCollideBullet(false);
                        if (flag3) {
                            this.isSkillEventEnd = false;
                            this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
                            ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 1, this.SKL1_END_TRIGGER, this.SKL1_END, OrangeCharacter.SubStatus.SKILL1_3, out this.skillEventFrame, out this.endFrame);
                            ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)70U, (HumanBase.AnimateId)70U, (HumanBase.AnimateId)72U, true);
                            return;
                        }
                        this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_MISS_BREAK;
                        int num = this._refEntity.IsInGround ? this.SKL1_END_MISS : this.SKL1_END_MISS_JUMP;
                        ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, 1, num, num, OrangeCharacter.SubStatus.SKILL1_2, out this.skillEventFrame, out this.endFrame);
                        this._refEntity.SetAnimateId((HumanBase.AnimateId)71U);
                        return;
                    }
                    break;
                }
                case OrangeCharacter.SubStatus.SKILL1_2:
                    if (this.nowFrame >= this.endFrame) {
                        this.OnSkillEnd();
                        return;
                    }
                    if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
                        ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
                        return;
                    }
                    break;
                case OrangeCharacter.SubStatus.SKILL1_3:
                    if (this.nowFrame >= this.endFrame) {
                        this.OnSkillEnd();
                        return;
                    }
                    if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
                        this.isSkillEventEnd = true;
                        WeaponStruct weaponStruct2 = this._refEntity.PlayerSkills[1];
                        this._refEntity.PushBulletDetail(this.linkSkl1, weaponStruct2.weaponStatus, this._refEntity.ModelTransform, weaponStruct2.SkillLV);
                        return;
                    }
                    if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
                        ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
                    }
                    break;
                default:
                    return;
                }
            }
            else {
                if (this.nowFrame >= this.endFrame) {
                    this.OnSkillEnd();
                    return;
                }
                if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
                    this.isSkillEventEnd = true;
                    WeaponStruct weaponStruct3 = this._refEntity.PlayerSkills[0];
                    ManagedSingleton<CharacterControlHelper>.Instance.UpdateShootDirByAimDir(this._refEntity);
                    ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct3.ShootTransform[0], MagazineType.ENERGY, -1, 1, true);
                    return;
                }
                if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame && (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT))) {
                    this.endFrame = this.nowFrame + 1;
                    return;
                }
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
        this.RecoverRushCollideBullet(true);
        HumanBase.AnimateId animateID = this._refEntity.AnimateID;
        if (animateID != HumanBase.AnimateId.ANI_SKILL_START) {
            if (this._refEntity.IsInGround) {
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
        this.RecoverRushCollideBullet(true);
        this.isSkillEventEnd = false;
        this._refEntity.IgnoreGravity = false;
        this._refEntity.SkillEnd = true;
        this._refEntity.CurrentActiveSkill = -1;
        this._refEntity.EnableCurrentWeapon();
    }

    public override void SetStun(bool enable) {
        // this.CallBase<CharacterControlBase, System.Action<bool>>("SetStun", enable); // base.SetStun(enable);
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

    private float GetRushSpd() {
        return (float)OrangeCharacter.DashSpeed * 4f;
    }

    private void UpdateVirtualButtonAnalog() {
        if (!this._refEntity.IsLocalPlayer) {
            return;
        }
        if (this._refPlayer != null) {
            this._refPlayer.SetVirtualButtonAnalog(VirtualButtonId.SKILL1, true);
        }
    }

    private void RushSkillHitCB(Il2CppSystem.Object obj) {
        if (!this._refEntity.IsLocalPlayer && !this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
            return;
        }
        if (this._refEntity.UsingVehicle) {
            this.RecoverRushCollideBullet(true);
            return;
        }
        Collider2D collider2D = obj.Cast<Collider2D>();
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
        
        // OrangeCharacter orangeCharacter = stageObjParam.tLinkSOB as OrangeCharacter;
        // EnemyControllerBase enemyControllerBase = stageObjParam.tLinkSOB as EnemyControllerBase;
        OrangeCharacter orangeCharacter = null;
        EnemyControllerBase enemyControllerBase = null;
        StageObjBase SOB = stageObjParam.tLinkSOB;
        try { orangeCharacter = SOB.Cast<OrangeCharacter>(); } catch ( Exception e ) {}
        try { enemyControllerBase = SOB.Cast<EnemyControllerBase>(); } catch ( Exception e ) {}

        if (orangeCharacter || enemyControllerBase) {
            this.RecoverRushCollideBullet(false);
            if (this._refEntity.IsLocalPlayer) {
                if (orangeCharacter) {
                    this._refEntity.selfBuffManager.AddBuff(-1, 0, 0, 0, false, orangeCharacter.sPlayerID, 0);
                    this._pRushTarget = new IAimTarget(orangeCharacter.Pointer);
                }
                else {
                    this._refEntity.selfBuffManager.AddBuff(-1, 0, 0, 0, false, "", 0);
                    this._pRushTarget = new IAimTarget(enemyControllerBase.Pointer);
                }
                this._tfHitTransform = transform;
            }
        }
    }

    private void UpdateSkill1Direction(float dirX) {
        if (Mathf.Abs(dirX) > 0.05f) {
            int num = Math.Sign(dirX);
            if (num != this._refEntity.direction) {
                this._refEntity.direction = num;
            }
        }
    }

    public override void SetRushBullet(RushCollideBullet rushCollideBullet) {
        this._pRushCollideBullet = rushCollideBullet;
        if (this._refEntity.UsingVehicle) {
            this._pRushTarget = null;
            this.RecoverRushCollideBullet(true);
        }
    }

    public override void SyncSkillDirection(Vector3 dir, IAimTarget target) {
        if (this._refEntity.UsingVehicle) {
            this._pRushTarget = null;
            this.RecoverRushCollideBullet(true);
            return;
        }
        this._bSyncRushSkillCompleted = true;
        this._vSyncDirection = dir;
        this._pRushTarget = target;
        if (this._refEntity.CurMainStatus == OrangeCharacter.MainStatus.SKILL && this._refEntity.CurSubStatus == OrangeCharacter.SubStatus.SKILL1) {
            this._vSkillVelocity = this._vSyncDirection.normalized * this.GetRushSpd();
            this.UpdateSkill1Direction(this._vSkillVelocity.x);
        }
    }

    protected void RecoverRushCollideBullet(bool claerBuff = false) {
        if (claerBuff && this._refEntity.IsLocalPlayer && this._refEntity.selfBuffManager.CheckHasEffectByCONDITIONID(-1, 0)) {
            this._refEntity.selfBuffManager.RemoveBuffByCONDITIONID(-1, true);
        }
        if (this._pRushCollideBullet) {
            this._pRushCollideBullet.BackToPool();
            this._pRushCollideBullet.HitCallback = null;
            this._pRushCollideBullet = null;
        }
    }

    public override Il2CppStringArray GetCharacterDependAnimations() {
        return new Il2CppStringArray(
            new string[] {
                "ch130_skill_01_crouch",
                "ch130_skill_01_stand",
                "ch130_skill_01_jump",
                "ch130_skill_02_start",
                "ch130_skill_02_loop",
                "ch130_skill_02_end",
                "ch130_skill_02_end_miss",
                "ch130_skill_02_jump_end"
            }
        );
    }

    private int nowFrame;

    private int skillEventFrame;

    private int endFrame;

    private int endBreakFrame;

    private bool isSkillEventEnd;

    private SKILL_TABLE linkSkl1;

    protected IAimTarget _pRushTarget;

    protected Transform _tfHitTransform;

    protected Vector3 _vSkillStartPosition;

    protected Vector2 _vSkillVelocity;

    protected RushCollideBullet _pRushCollideBullet;

    protected bool _bSyncRushSkillCompleted;

    protected Vector3 _vSyncDirection;

    private int sklFrenzyId = 22661;

    private int conditionId = -1;

    private SKILL_TABLE linkSklPassive;

    protected ShieldBullet _pShieldBullet;

    private ParticleSystem _regularEffect;

    private ParticleSystem[] _FrenzyEffect;

    private bool isInit;

    private OrangeConsoleCharacter _refPlayer;

    private readonly string FX_0_00 = "fxuse_blackbombs_000";

    private readonly string FX_1_00 = "fxuse_GoreICO_000";

    private readonly string FX_2_00 = "fxhit_barrage_003";

    protected readonly int SKL0_TRIGGER = (int)(0.292f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL0_END = (int)(0.833f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL0_END_BREAK = (int)(0.62f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_START = (int)(0.067f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_LOOP = (int)(0.35 / (double)GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END = (int)(1f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END_TRIGGER = (int)(0.26f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END_BREAK = (int)(0.9f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END_MISS = (int)(0.6f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END_MISS_JUMP = (int)(0.45f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL1_END_MISS_BREAK = (int)(0.33f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int FRENZY_TIMER = (int)(2f / GameLogicUpdateManager.m_fFrameLen);
}
