
using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH141_Controller : CharacterControlBase_ {

    public CH141_Controller() : base() {}
    
    public CH141_Controller(IntPtr p) : base(p) {}
    
    private void OnEnable() {
        MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.AddUpdate(new ILogicUpdate(this.Pointer));
    }

    private void OnDisable() {
        MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.RemoveUpdate(new ILogicUpdate(this.Pointer));
    }

    public override void Start() {
        base.Start();
        this.InitializeSkill();
    }

    private void InitializeSkill() {
        Transform transform = new GameObject("CustomShootPoint0").transform;
        transform.SetParent(base.transform);
        transform.localPosition = new Vector3(0f, 0.8f, 0f);
        Il2CppReferenceArray<Transform> componentsInChildren = this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
        this._refEntity.ExtraTransforms = new Transform[1];
        this._refEntity.ExtraTransforms[0] = transform;
        this._refEntity.PlayerSkills[0].ShootTransform[0] = this._refEntity.ExtraTransforms[0];
        this._refEntity.PlayerSkills[1].ShootTransform[0] = this._refEntity.ExtraTransforms[0];
        this._FrenzyEffect = new ParticleSystem[1];
        this._FrenzyEffect[0] = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxduring_DMCZDevilTrigger_000_(work)", false).GetComponent<ParticleSystem>();
        this._FrenzyEffect[0].gameObject.SetActive(false);
        this._teleportEffect = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxdemo_DMCZ_003_(work)", false).GetComponent<ParticleSystem>();
        this._saberEffect = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "fxdemo_DMCZ_004_(work)", false).GetComponent<ParticleSystem>();
        GameObject gameObject = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "Hand_KatanaBladeMesh_A_m", true).gameObject;
        if (gameObject) {
            this.cmSaber = gameObject.GetComponent<CharacterMaterial>();
        }
        GameObject gameObject2 = OrangeBattleUtility.FindChildRecursive(componentsInChildren, "Waist_KatanaHandleMesh_m", true).gameObject;
        if (gameObject2) {
            this.cmSaberSide = gameObject2.GetComponent<CharacterMaterial>();
        }
        ManagedSingleton<CharacterControlHelper>.Instance.PreloadLinkSkl<BulletBase>(this._refEntity, 1, out this.linkSkl1_0);
        if (this.linkSkl1_0 != null && this.linkSkl1_0.n_COMBO_SKILL != 0 && ManagedSingleton<OrangeDataManager>.Instance.SKILL_TABLE_DICT.TryGetNewValue(this.linkSkl1_0.n_COMBO_SKILL, out this.linkSkl1_1)) {
            this._refEntity.tRefPassiveskill.ReCalcuSkill(ref this.linkSkl1_1);
        }
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_0_00, 2, null);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_00, 2, null);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_1_01, 2, null);
        MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.FX_2_00, 2, null);
        this.isInit = true;
    }

    public override void OverrideDelegateEvent() {
        this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
        this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
        this._refEntity.PlayTeleportOutEffectEvt = (Callback)new Action(this.PlayTeleportOutEffect);
        this._refEntity.TeleportOutCharacterDependEvt = (Callback)new Action(this.TeleportOutCharacterDepend);
        this._refEntity.SetStatusCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.SetStatusCharacterDepend);
        this._refEntity.AnimationEndCharacterDependEvt = new Action<OrangeCharacter.MainStatus, OrangeCharacter.SubStatus>(this.AnimationEndCharacterDepend);
        this._refEntity.StageTeleportInCharacterDependEvt = (Callback)new Action(this.StageTeleportInCharacterDepend);
        this._refEntity.StageTeleportOutCharacterDependEvt = (Callback)new Action(this.StageTeleportOutCharacterDepend);
    }

    private void PlayTeleportOutEffect() {
        Vector3 p_worldPos = base.transform.position;
        if (this._refEntity != null) {
            p_worldPos = this._refEntity.AimPosition;
        }
        FxManager_.Play("FX_TELEPORT_OUT", p_worldPos, Quaternion.identity);
    }

    protected void StageTeleportInCharacterDepend() {
        if (this.cmSaberSide) {
            this.cmSaberSide.Appear(null, -1f);
        }
    }

    protected void StageTeleportOutCharacterDepend() {
        if (this.cmSaberSide) {
            this.cmSaberSide.Disappear(null, -1f);
        }
    }

    public void LogicUpdate() {
        if (this.isInit) {
            this.CheckFrenzyBuff();
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
                this.StopFrenzyFx();
            }
        }
    }

    private void PlayFrenzyFx() {
        this.isFrenzyStatus = true;
        this._FrenzyEffect[0].gameObject.SetActive(true);
        this._FrenzyEffect[0].Play(true);
        FxManager_.Play(this.FX_2_00, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
        base.PlaySkillSE("zc_majin01");
    }

    private void StopFrenzyFx() {
        this.isFrenzyStatus = false;
        this._FrenzyEffect[0].gameObject.SetActive(false);
        this._FrenzyEffect[0].Stop(true);
        base.PlaySkillSE("zc_majin02");
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
            this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
            WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
            OrangeBattleUtility.UpdateSkillCD(weaponStruct);
            this._refEntity.CheckUsePassiveSkill(1, weaponStruct.weaponStatus, weaponStruct.ShootTransform[0]);
            ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_START_END, this.SKL1_START_END, OrangeCharacter.SubStatus.SKILL1, out this.skillEventFrame, out this.endFrame);
            this.UseSkill1();
            ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)68U, (HumanBase.AnimateId)68U, (HumanBase.AnimateId)71U, true);
            return;
        }
        else {
            if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
                return;
            }
            this.isSkillEventEnd = false;
            this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
            this._refEntity.IsShoot = 0;
            ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_TRIGGER, this.SKL0_END, OrangeCharacter.SubStatus.SKILL0, out this.skillEventFrame, out this.endFrame);
            ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)67U, true);
            this._refEntity.DisableWeaponMesh(this._refEntity.GetCurrentWeaponObj(), 0.01f);
            WeaponStruct weaponStruct2 = this._refEntity.PlayerSkills[0];
            OrangeBattleUtility.UpdateSkillCD(weaponStruct2);
            this._refEntity.CheckUsePassiveSkill(0, weaponStruct2.weaponStatus, weaponStruct2.ShootTransform[0]);
            FxManager_.Play(this.FX_0_00, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
            base.PlayVoiceSE("v_zc_skill01");
            base.PlaySkillSE("zc_earth01");
            return;
        }
    }

    public override void PlayerReleaseSkillCharacterCall(int id) {
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
            if (curSubStatus != OrangeCharacter.SubStatus.SKILL0) {
                switch (curSubStatus) {
                case OrangeCharacter.SubStatus.SKILL1:
                    break;
                case OrangeCharacter.SubStatus.SKILL1_1:
                    if (this.nowFrame >= this.endFrame) {
                        this.SetStatusSkill_2();
                        return;
                    }
                    break;
                case OrangeCharacter.SubStatus.SKILL1_2:
                    if (this.nowFrame >= this.endFrame) {
                        this._refEntity.BulletCollider.BackToPool();
                        this.OnSkillEnd();
                    }
                    else if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
                        this.isSkillEventEnd = true;
                        if (this.isFrenzyStatus) {
                            if (this.linkSkl1_1 != null) {
                                this.PushLinkSkl(this.linkSkl1_1, this._refEntity.PlayerSkills[1].ShootTransform[0].position);
                            }
                        }
                        else if (this.linkSkl1_0 != null) {
                            this.PushLinkSkl(this.linkSkl1_0, this._refEntity.PlayerSkills[1].ShootTransform[0].position);
                        }
                    }
                    else if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
                        ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endBreakFrame);
                    }
                    if (this.nowFrame == this.seEventFrame) {
                        base.PlaySkillSE("zc_jigen03");
                        return;
                    }
                    if (this.nowFrame == this.fxEventFrame) {
                        FxManager_.Play(this.FX_1_01, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
                        if (this._saberEffect) {
                            this._saberEffect.Play(true);
                        }
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
                    WeaponStruct weaponStruct = this._refEntity.PlayerSkills[0];
                    ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, weaponStruct.ShootTransform[0], MagazineType.NORMAL, this._refEntity.GetCurrentSkillObj().Reload_index, 0, false);
                    base.PlaySkillSE("zc_earth02");
                    return;
                }
                if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
                    ManagedSingleton<CharacterControlHelper>.Instance.CheckBreakFrame(this._refEntity.UserID, ref this.endFrame);
                    return;
                }
            }
        }
    }

    private void UseSkill1() {
        PlayerAutoAimSystem playerAutoAimSystem = this._refEntity.PlayerAutoAimSystem;
        IAimTarget aimTarget = playerAutoAimSystem.AutoAimTarget;
        WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
        if (this.IsPVPMode && aimTarget != null) {
            float magnitude = (aimTarget.AimPosition - this._refEntity.AimPosition).magnitude;
            if (!playerAutoAimSystem.IsInsideScreenExactly(aimTarget.AimPosition) || magnitude > weaponStruct.BulletData.f_DISTANCE) {
                aimTarget = null;
            }
        }
        this._targetPos = null;
        this._isTeleporation = false;
        this.IsSkl1InGround = this._refEntity.IsInGround;

        if (this.IsPVPMode) {
            if (aimTarget != null) {
                this._targetPos = new Vector3?(aimTarget.AimPosition);
                this._isTeleporation = true;
                OrangeCharacter orangeCharacter = new OrangeCharacter(aimTarget.Pointer);
                if (orangeCharacter != null) {
                    this._targetPos = new Vector3?(orangeCharacter._transform.position);
                }
            }
        }
        else {
            float f_DISTANCE = weaponStruct.BulletData.f_DISTANCE;
            Vector3 aimPosition;
            if (aimTarget == null) {
                if (this.IsSkl1InGround) {
                    aimPosition = new Vector3(this._refEntity.AimPosition.x, this._refEntity.transform.position.y, this._refEntity.AimPosition.z);
                }
                else {
                    aimPosition = this._refEntity.AimPosition;
                }
                aimPosition.x += Mathf.Sign(this._refEntity.ShootDirection.x) * f_DISTANCE;
            }
            else {
                aimPosition = aimTarget.AimPosition;
                if (this.IsSkl1InGround && Mathf.Abs(aimPosition.y - this._refEntity.AimPosition.y) > 0.2f) {
                    this.IsSkl1InGround = false;
                }
            }
            this._targetPos = new Vector3?(Vector3.MoveTowards(this._refEntity.AimPosition, aimPosition, f_DISTANCE));
        }
        if (this._targetPos != null) {
            int num = Math.Sign((this._targetPos.Value - this._refEntity.AimPosition).normalized.x);
            this._refEntity.direction = ((num != 0) ? num : this._refEntity.direction);
        }
        FxManager_.Play(this.FX_1_00, this._refEntity.ModelTransform.position, OrangeCharacter.NormalQuaternion);
        base.PlayVoiceSE("v_zc_skill02");
        base.PlaySkillSE("zc_jigen01");
    }

    private void SetStatusSkill_2() {
        this.isSkillEventEnd = false;
        this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
        this.seEventFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_SE_TRIGGER;
        this.fxEventFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_FX_TRIGGER;
        ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL1_END_TRIGGER, this.SKL1_END_END, OrangeCharacter.SubStatus.SKILL1_2, out this.skillEventFrame, out this.endFrame);
    }

    public void SetStatusCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
        if (mainStatus != OrangeCharacter.MainStatus.TELEPORT_OUT) {
            if (mainStatus == OrangeCharacter.MainStatus.SKILL) {
                switch (subStatus) {
                case OrangeCharacter.SubStatus.SKILL1:
                    break;
                case OrangeCharacter.SubStatus.SKILL1_1: {
                    if (this._targetPos != null) {
                        Vector3 a = this._targetPos.Value - this._refEntity.AimPosition;
                        if (this._isTeleporation) {
                            Vector3 value = this._targetPos.Value;
                            if (this._refEntity.IsLocalPlayer) {
                                this._refEntity.Controller.LogicPosition = new VInt3(value);
                                this._refEntity.transform.position = value;
                            }
                        }
                        else {
                            VInt2 vint = new VInt2(a / OrangeBattleUtility.PPU / OrangeBattleUtility.FPS / GameLogicUpdateManager.m_fFrameLen);
                            this._refEntity.SetSpeed(vint.x, vint.y);
                            if (this.IsSkl1InGround) {
                                this._refEntity.SetAnimateId((HumanBase.AnimateId)69U);
                            }
                            else {
                                this._refEntity.SetAnimateId((HumanBase.AnimateId)72U);
                            }
                        }
                    }
                    int num = 0;
                    this.endFrame = this.nowFrame + num;
                    return;
                }
                case OrangeCharacter.SubStatus.SKILL1_2: {
                    this._refEntity.SetSpeed(0, 0);
                    WeaponStruct weaponStruct = this._refEntity.PlayerSkills[1];
                    BulletBase bulletCollider = this._refEntity.BulletCollider;
                    SKILL_TABLE bulletData = weaponStruct.BulletData;
                    string sPlayerName = this._refEntity.sPlayerName;
                    int nowRecordNO = this._refEntity.GetNowRecordNO();
                    OrangeCharacter refEntity = this._refEntity;
                    int nBulletRecordID = refEntity.nBulletRecordID;
                    refEntity.nBulletRecordID = nBulletRecordID + 1;
                    bulletCollider.UpdateBulletData(bulletData, sPlayerName, nowRecordNO, nBulletRecordID, 1);
                    this._refEntity.BulletCollider.SetBulletAtk(weaponStruct.weaponStatus, this._refEntity.selfBuffManager.sBuffStatus, null);
                    this._refEntity.BulletCollider.BulletLevel = weaponStruct.SkillLV;
                    this._refEntity.BulletCollider.Active(this._refEntity.TargetMask);
                    this._refEntity.BulletCollider.SetDirection((float)this._refEntity.direction * Vector3.right);
                    if (this.IsSkl1InGround) {
                        this._refEntity.SetAnimateId((HumanBase.AnimateId)70U);
                    }
                    else {
                        this._refEntity.SetAnimateId((HumanBase.AnimateId)73U);
                    }
                    this.ToggleSaber(true);
                    return;
                }
                default:
                    return;
                }
            }
        }
        else {
            OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
            if (curSubStatus != OrangeCharacter.SubStatus.TELEPORT_POSE) {
                if (curSubStatus == OrangeCharacter.SubStatus.WIN_POSE) {
                    this.isWinPose = true;
                    this.winFxFrame = GameLogicUpdateManager.GameFrame + this.WIN_TRIGGER;
                    this.ToggleSaber(true);
                    this.SetSaberToEntitySub();
                    return;
                }
            }
            else {
                if (this.isWinPose) {
                    this.logoutFxFrame = GameLogicUpdateManager.GameFrame + this.LOGOUT_TRIGGER;
                }
                this.SetSaberToEntitySub();
            }
        }
    }

    protected void AnimationEndCharacterDepend(OrangeCharacter.MainStatus mainStatus, OrangeCharacter.SubStatus subStatus) {
        if (mainStatus == OrangeCharacter.MainStatus.SKILL && subStatus == OrangeCharacter.SubStatus.SKILL1) {
            this._refEntity.IgnoreGravity = true;
            if (this._isTeleporation && this._targetPos == null) {
                this.SetStatusSkill_2();
                return;
            }
            ManagedSingleton<CharacterControlHelper>.Instance.ChangeToNextStatus(this._refEntity, this.SKL1_LOOP_END, this.SKL1_LOOP_END, OrangeCharacter.SubStatus.SKILL1_1, out this.skillEventFrame, out this.endFrame);
        }
    }

    private void TeleportOutCharacterDepend() {
        OrangeCharacter.SubStatus curSubStatus = this._refEntity.CurSubStatus;
        if (curSubStatus != OrangeCharacter.SubStatus.TELEPORT_POSE) {
            if (curSubStatus == OrangeCharacter.SubStatus.WIN_POSE && GameLogicUpdateManager.GameFrame == this.winFxFrame && this._teleportEffect) {
                this._teleportEffect.Play(true);
                return;
            }
        }
        else if (GameLogicUpdateManager.GameFrame == this.logoutFxFrame && this._saberEffect) {
            this._saberEffect.Play(true);
        }
    }

    private void PushLinkSkl(SKILL_TABLE bulletData, Vector3 shootPosition, Il2CppSystem.Nullable_Unboxed<Vector3> ShotDir = default(Il2CppSystem.Nullable_Unboxed<Vector3>)) {
        WeaponStruct currentSkillObj = this._refEntity.GetCurrentSkillObj();
        this._refEntity.PushBulletDetail(bulletData, currentSkillObj.weaponStatus, shootPosition, currentSkillObj.SkillLV, ShotDir);
    }

    private void OnSkillEnd() {
        if (this._refEntity.IgnoreGravity) {
            this._refEntity.IgnoreGravity = false;
        }
        this.isSkillEventEnd = false;
        this._refEntity.SkillEnd = true;
        this._refEntity.CurrentActiveSkill = -1;
        this._refEntity.EnableCurrentWeapon();
        this.ToggleSaber(false);
        HumanBase.AnimateId animateID = this._refEntity.AnimateID;
        if (this._refEntity.IsInGround) {
            this._refEntity.Dashing = false;
            this._refEntity.SetSpeed(0, 0);
            this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
            return;
        }
        this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
    }

    private void ToggleSaber(bool enable) {
        if (this.cmSaber) {
            if (enable) {
                this.cmSaber.Appear(null, -1f);
                return;
            }
            this.cmSaber.Disappear(null, -1f);
        }
    }

    private void ToggleSideSaber(bool enable) {
        if (this.cmSaberSide) {
            if (enable) {
                this.cmSaberSide.Appear(null, -1f);
                return;
            }
            this.cmSaberSide.Disappear(null, -1f);
        }
    }

    private void SetSaberToEntitySub() {
        if (this.cmSaber && this.cmSaberSide && this.cmSaber.GetSubCharacterMaterials == null) {
            this.cmSaber.ChangeDissolveTime(0.8f);
            this.cmSaberSide.ChangeDissolveTime(0.8f);
            this._refEntity.CharacterMaterials.SetSubCharacterMaterial(this.cmSaber);
            this.cmSaber.SetSubCharacterMaterial(this.cmSaberSide);
        }
    }

    public override void ClearSkill() {
        this.isSkillEventEnd = false;
        this._refEntity.IgnoreGravity = false;
        this._refEntity.SkillEnd = true;
        this._refEntity.CurrentActiveSkill = -1;
        this._refEntity.EnableCurrentWeapon();
        this.ToggleSaber(false);
        this.ToggleSideSaber(true);
        if (this._refEntity.BulletCollider.IsActivate) {
            this._refEntity.BulletCollider.BackToPool();
        }
    }

    public override void SetStun(bool enable) {
        // this.CallBase<CharacterControlBase, System.Action<bool>>("SetStun", enable); // base.SetStun(enable);
        this.ToggleSaber(false);
        this.ToggleSideSaber(true);
        this._refEntity.EnableCurrentWeapon();
    }

    public override void ControlCharacterDead() {
        this.ToggleSaber(false);
        this.ToggleSideSaber(false);
    }

    public override void ControlCharacterContinue() {
        this.ToggleSideSaber(true);
    }

    // (get) Token: 0x060039AA RID: 14762 RVA: 0x00038B79 File Offset: 0x00036D79
    private bool IsPVPMode {
        get {
            return MonoBehaviourSingleton<OrangeBattleServerManager>.Instance.IsPvp;
        }
    }

    public override Il2CppStringArray GetCharacterDependAnimations() {
        return new Il2CppStringArray(
            new string[] {
                "ch141_skill_01_crouch",
                "ch141_skill_01_stand",
                "ch141_skill_01_jump",
                "ch141_skill_02_stand_start",
                "ch141_skill_02_stand_loop",
                "ch141_skill_02_stand_end",
                "ch141_skill_02_jump_start",
                "ch141_skill_02_jump_loop",
                "ch141_skill_02_jump_end"
            }
        );
    }

    private int nowFrame;

    private int skillEventFrame;

    private int endFrame;

    private int endBreakFrame;

    private bool isSkillEventEnd;

    private SKILL_TABLE linkSkl1_0;

    private SKILL_TABLE linkSkl1_1;

    private bool isFrenzyStatus;

    private ParticleSystem[] _FrenzyEffect;

    private ParticleSystem _teleportEffect;

    private ParticleSystem _saberEffect;

    private bool isInit;

    private CharacterMaterial cmSaber;

    private CharacterMaterial cmSaberSide;

    private Vector3? _targetPos;

    private bool _isTeleporation;

    private int seEventFrame;

    private int fxEventFrame;

    private bool IsSkl1InGround = true;

    private bool isWinPose;

    private int winFxFrame = -1;

    private int logoutFxFrame = -1;

    private readonly string FX_0_00 = "fxuse_DMCZHell_000";

    private readonly string FX_1_00 = "fxuse_DMCZJudgmentCutEnd_000";

    private readonly string FX_1_01 = "fxuse_DMCZJudgmentCutEnd_001";

    private readonly string FX_2_00 = "fxuse_DMCZDevilTrigger_000";

    protected readonly int SKL0_TRIGGER = (int)(0.369f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL0_END = (int)(1f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int SKL0_END_BREAK = (int)(0.5f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_START_END = (int)(0.167f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_LOOP_END = (int)(0.417f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_END_END = (int)(2f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_END_TRIGGER = 1;

    private readonly int SKL1_END_SE_TRIGGER = (int)(1.18f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_END_FX_TRIGGER = (int)(1.4f / GameLogicUpdateManager.m_fFrameLen);

    private readonly int SKL1_END_BREAK = (int)(1.6f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int WIN_TRIGGER = (int)(0.3f / GameLogicUpdateManager.m_fFrameLen);

    protected readonly int LOGOUT_TRIGGER = (int)(0.46f / GameLogicUpdateManager.m_fFrameLen);
}
