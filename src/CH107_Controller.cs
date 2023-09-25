using System;
using MagicaCloth;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CH107_CharacterMaterial : CharacterMaterial {

	public CH107_CharacterMaterial() : base() {
		this.OutlineColor = new Color(0.142f, 0.168f, 0.227f);
	}
	
	public CH107_CharacterMaterial(IntPtr p) : base(p) {
		this.OutlineColor = new Color(0.142f, 0.168f, 0.227f);
	}

	public override void UpdatePropertyBlock() {
		this.UpdateColor();
		this.UpdateDissolve();
		this.CallBase<CharacterMaterial>("UpdatePropertyBlock");
	}

	private void Start() {
		this.UpdatePropertyBlock();
	}

	private void UpdateColor() {
		if (this.canChangeColor) {
			OrangeMaterialProperty instance = MonoBehaviourSingleton<OrangeMaterialProperty>.Instance;
			if (base.gameObject.layer == ManagedSingleton<OrangeLayerManager>.Instance.DefaultLayer) {
				this.mpb.SetColor(instance.i_TintColor, this.colors[0]);
				return;
			}
			this.mpb.SetColor(instance.i_TintColor, this.colors[1]);
		}
	}

	private void UpdateDissolve() {
		if (this.canChangeDissolve) {
			OrangeMaterialProperty instance = MonoBehaviourSingleton<OrangeMaterialProperty>.Instance;
			float a = 1f - this.mpb.GetFloat(instance.i_DissolveValue);
			float @float = this.mpb.GetFloat(instance.i_Intensity);
			this.mpb.SetFloat(instance.i_AlphaValue, Mathf.Min(a, @float));
		}
	}

	// [SerializeField]
	private bool canChangeColor;

	// [SerializeField]
	private bool canChangeDissolve;

	private const float factor = 3f;

	private Color[] colors = new Color[] {
		new Color(2.035294f, 2.2470589f, 2.2470589f, 1f),
		new Color(1.5411766f, 2.2470589f, 2.2117648f, 1f)
	};
}


public class CH107_Controller : CharacterControlBase {

	public CH107_Controller() : base() {}
	
	public CH107_Controller(IntPtr p) : base(p) {}
    
	private void OnEnable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.AddUpdate(new ILogicUpdate(this.Pointer));
	}

	private void OnDisable() {
		MonoBehaviourSingleton<GameLogicUpdateManager>.Instance.RemoveUpdate(new ILogicUpdate(this.Pointer));
	}

	public override void Start() {
		this.CallBase<CharacterControlBase>("Start"); // base.Start();
		this.InitializeSkill();
	}

	public override void OverrideDelegateEvent() {
		this.CallBase<CharacterControlBase>("OverrideDelegateEvent"); // base.OverrideDelegateEvent();
	}

	private void InitializeSkill() {
		// this._refEntity._transform.GetComponentsInChildren<Transform>(true).Cast<Il2CppReferenceArray<Transform>>();
		Transform transform = new GameObject(this.sCustomShootPoint + "0").transform;
		transform.SetParent(base.transform);
		transform.localPosition = new Vector3(0f, 2.1f, 0f);
		Transform transform2 = new GameObject(this.sCustomShootPoint + "1").transform;
		transform2.SetParent(base.transform);
		transform2.localPosition = this.skl1Offset;
		transform2.transform.localScale = new Vector3(1f, 1f, 1f);
		this._refEntity.ExtraTransforms = new Transform[] {
			transform,
			transform2
		};
		CharacterMaterial[] components = this._refEntity.CharacterMaterials.gameObject.GetComponents<CharacterMaterial>();
		for (int i = 0; i < components.Length; i++) {
			if (components[i].GetRenderer().Length == 1) {
				if (components[i].GetRenderer()[0].name == "HairMesh_01_c") {
					this.hairNormal = components[i];
				}
				else if (components[i].GetRenderer()[0].name == "HairMesh_02_c") {
					this.hairLightning = components[i];
				}
			}
			else {
				this.body = components[i];
			}
		}
		this.boneCloths = this._refEntity.CharacterMaterials.gameObject.GetComponentsInChildren<MagicaBoneCloth>();
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fx_teleportIn, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fx_teleportOut, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fx_teleportOut2, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fx_lightning, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fx_lightning_stop, 1, null);
		MonoBehaviourSingleton<FxManager>.Instance.PreloadFx(this.fxuse_skl1, 1, null);
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
			base.PlayVoiceSE("v_ir3_skill02");
			base.PlaySkillSE("ir3_raigeki");
			this._refEntity.CurrentActiveSkill = id;
			ManagedSingleton<CharacterControlHelper>.Instance.TurnToAimTarget(this._refEntity);
			this._refEntity.ExtraTransforms[1].localPosition = this.skl1Offset * (float)this._refEntity._characterDirection;
			int reload_index = this._refEntity.GetCurrentSkillObj().Reload_index;
			OrangeCharacter.SubStatus p_nextStatus;
			if (reload_index != 1) {
				p_nextStatus = OrangeCharacter.SubStatus.SKILL1;
			}
			else {
				p_nextStatus = OrangeCharacter.SubStatus.SKILL1_1;
			}
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL1_END_BREAK;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL1_TRIGGER, this.SKL1_END, p_nextStatus, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, (HumanBase.AnimateId)68U, (HumanBase.AnimateId)69U, (HumanBase.AnimateId)70U, true);
			this.SetBoneClothWeight(0f);
			return;
		}
		else {
			if (!this._refEntity.CheckUseSkillKeyTrigger(id, true)) {
				return;
			}
			base.PlayVoiceSE("v_ir3_skill01");
			this._refEntity.CurrentActiveSkill = id;
			ManagedSingleton<CharacterControlHelper>.Instance.TurnToAimTarget(this._refEntity);
			int reload_index = this._refEntity.GetCurrentSkillObj().Reload_index;
			OrangeCharacter.SubStatus p_nextStatus2;
			if (reload_index != 1) {
				p_nextStatus2 = OrangeCharacter.SubStatus.SKILL0;
			}
			else {
				p_nextStatus2 = OrangeCharacter.SubStatus.SKILL0_1;
			}
			this.fxUseSkl2 = FxManager_.PlayReturn<FxBase>(this.fxuse_skl1, this._refEntity.ModelTransform, Quaternion.identity);
			this.endBreakFrame = GameLogicUpdateManager.GameFrame + this.SKL0_END_BREAK;
			ManagedSingleton<CharacterControlHelper>.Instance.ChangeToSklStatus(this._refEntity, id, this.SKL0_TRIGGER, this.SKL0_END, p_nextStatus2, out this.skillEventFrame, out this.endFrame);
			ManagedSingleton<CharacterControlHelper>.Instance.SetAnimate(this._refEntity, HumanBase.AnimateId.ANI_SKILL_START, (HumanBase.AnimateId)66U, (HumanBase.AnimateId)67U, true);
			return;
		}
	}

	public void LogicUpdate() {
		this.CheckLightninBuff();
		PerBuffManager refPBM = this._refEntity.selfBuffManager.sBuffStatus.refPBM;
		if (!this.spIsFull) {
			if (refPBM.nMeasureNow == refPBM.nMeasureMax) {
				this.spIsFull = true;
				return;
			}
		}
		else if (refPBM.nMeasureNow != refPBM.nMeasureMax) {
			this.spIsFull = false;
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
			if (curSubStatus - OrangeCharacter.SubStatus.SKILL0 > 1) {
				if (curSubStatus - OrangeCharacter.SubStatus.SKILL1 > 1) {
					return;
				}
				if (this.nowFrame >= this.endFrame) {
					this.SetBoneClothWeight(1f);
					this.OnSkillEnd();
					return;
				}
				if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
					this.isSkillEventEnd = true;
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ExtraTransforms[1], MagazineType.ENERGY, this._refEntity.GetCurrentSkillObj().Reload_index, 0, true);
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
					this.CheckBreakFrame();
				}
			}
			else {
				if (this.nowFrame >= this.endFrame) {
					this.OnSkillEnd();
					return;
				}
				if (!this.isSkillEventEnd && this.nowFrame >= this.skillEventFrame) {
					this.isSkillEventEnd = true;
					this._refEntity.ShootDirection = Vector2.right * (float)this._refEntity.direction;
					ManagedSingleton<CharacterControlHelper>.Instance.PushBulletSkl(this._refEntity, this._refEntity.ExtraTransforms[0], MagazineType.ENERGY, this._refEntity.GetCurrentSkillObj().Reload_index, 0, true);
					return;
				}
				if (this.isSkillEventEnd && this.nowFrame >= this.endBreakFrame) {
					this.CheckBreakFrame();
					return;
				}
			}
		}
	}

	private void CheckLightninBuff() {
		if (this._refEntity.PlayerSkills.Length != 0) {
			if (this._refEntity.PlayerSkills[0].Reload_index == 1) {
				if (!this.isLightningStatus) {
					this.PlayLightningFx();
					return;
				}
			}
			else if (this.isLightningStatus) {
				this.StopLightningFx();
			}
		}
	}

	public override void ClearSkill() {
		this._refEntity.EnableCurrentWeapon();
		if (this.fxUseSkl2) {
			this.fxUseSkl2.BackToPool();
		}
		this.SetBoneClothWeight(1f);
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

	private void CheckBreakFrame() {
		if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.LEFT) || ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.RIGHT)) {
			this.endFrame = this.nowFrame + 1;
		}
	}

	private void OnSkillEnd() {
		this._refEntity.IgnoreGravity = false;
		this.isSkillEventEnd = false;
		this._refEntity.SkillEnd = true;
		this._refEntity.CurrentActiveSkill = -1;
		this._refEntity.EnableCurrentWeapon();
		switch (this._refEntity.AnimateID) {
		case HumanBase.AnimateId.ANI_SKILL_START:
		case (HumanBase.AnimateId)68U:
			this._refEntity.Dashing = false;
			if (ManagedSingleton<InputStorage>.Instance.IsHeld(this._refEntity.UserID, ButtonId.DOWN)) {
				this._refEntity.SetStatus(OrangeCharacter.MainStatus.CROUCH, OrangeCharacter.SubStatus.WIN_POSE);
				return;
			}
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.CROUCH_UP);
			return;
		case (HumanBase.AnimateId)66U:
		case (HumanBase.AnimateId)69U:
			this._refEntity.Dashing = false;
			this._refEntity.SetSpeed(0, 0);
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.IDLE, OrangeCharacter.SubStatus.IDLE);
			return;
		default:
			this._refEntity.SetStatus(OrangeCharacter.MainStatus.FALL, OrangeCharacter.SubStatus.TELEPORT_POSE);
			return;
		}
	}

	private void PlayLightningFx() {
		this.isLightningStatus = true;
		this._refEntity.CharacterMaterials.ClearSubCharacterMaterial();
		this._refEntity.CharacterMaterials.SetSubCharacterMaterial(this.hairLightning);
		this.hairLightning.Appear(null, 0.3f);
		this.hairNormal.Disappear(null, 1f);
		this.body.UpdateEmission(2f);
		this.RefreashLightningFx(true);
	}

	private void StopLightningFx() {
		this.isLightningStatus = false;
		this._refEntity.CharacterMaterials.ClearSubCharacterMaterial();
		this._refEntity.CharacterMaterials.SetSubCharacterMaterial(this.hairNormal);
		this.hairNormal.Appear(null, 0.3f);
		this.hairLightning.Disappear(null, 1f);
		this.body.UpdateEmission(0f);
		this.RefreashLightningFx(false);
	}

	private void RefreashLightningFx(bool play) {
		if (this.fxLightning != null) {
			this.fxLightning.BackToPool();
			this.fxLightning = null;
			if (!play) {
				FxManager_.Play(this.fx_lightning_stop, this._refEntity.ModelTransform, Quaternion.identity);
			}
		}
		if (play) {
			this.fxLightning = FxManager_.PlayReturn<FxBase>(this.fx_lightning, this._refEntity.ModelTransform, Quaternion.identity);
		}
	}

	private void SetBoneClothWeight(float val) {
		MagicaBoneCloth[] array = this.boneCloths;
		for (int i = 0; i < array.Length; i++) {
			array[i].BlendWeight = val;
		}
	}

	public override Il2CppStringArray GetCharacterDependAnimations() {
		return new Il2CppStringArray(
			new string[] {
				"ch107_skill_01_crouch",
				"ch107_skill_01_stand",
				"ch107_skill_01_jump",
				"ch107_skill_02_crouch",
				"ch107_skill_02_stand",
				"ch107_skill_02_jump"
			}
		);
	}

	private int nowFrame;

	private int skillEventFrame;

	private int endFrame;

	private bool isSkillEventEnd;

	private int endBreakFrame;

	private bool isLightningStatus;

	private Vector3 skl1Offset = new Vector3(1.5f, 0f, 0f);

	private FxBase fxLightning;

	private FxBase fxUseSkl2;

	private CharacterMaterial hairNormal;

	private CharacterMaterial hairLightning;

	private CharacterMaterial body;

	private MagicaBoneCloth[] boneCloths;

	private bool spIsFull = true;

	private readonly string sCustomShootPoint = "CustomShootPoint";

	private readonly string fx_teleportIn = "fxdemo_zinogre_003";

	private readonly string fx_teleportOut = "fxdemo_zinogre_004";

	private readonly string fx_teleportOut2 = "fxdemo_zinogre_005";

	private readonly string fx_lightning = "fxduring_zinogre_000";

	private readonly string fx_lightning_stop = "fxduring_zinogre_001";

	private readonly string fxuse_skl1 = "fxuse_lightbowgun_001";

	private readonly int SKL0_TRIGGER = (int)(0.13f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END = (int)(0.667f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL0_END_BREAK = (int)(0.333f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_TRIGGER = (int)(0.25f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_END = (int)(0.625f / GameLogicUpdateManager.m_fFrameLen);

	private readonly int SKL1_END_BREAK = (int)(0.3f / GameLogicUpdateManager.m_fFrameLen);
}
