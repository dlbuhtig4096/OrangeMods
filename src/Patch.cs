﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

using System;
using CallbackDefs;
using UnityEngine;
using StageLib;

[HarmonyPatch(typeof(PlayerStatus))]
class PlayerStatus_ {

    [HarmonyPrefix]
    [HarmonyPatch("op_Addition", typeof(PlayerStatus), typeof(WeaponStatus))]
    static bool op_Addition(PlayerStatus a, WeaponStatus b, ref PlayerStatus __result) {
        __result = new PlayerStatus {
			nHP = a.nHP + b.nHP,
			nATK = a.nATK + b.nATK,
			nDEF = a.nDEF + b.nDEF,
			nCRI = a.nCRI + b.nCRI,
			nHIT = a.nHIT + b.nHIT,
			nLuck = a.nLuck + b.nLuck,
			nDOD = a.nDOD,
			nLV = a.nLV,
			nCriDmgPercent = a.nCriDmgPercent + b.nCriDmgPercent,
			nReduceCriPercent = a.nReduceCriPercent + b.nReduceCriPercent,
			nBlockDmgPercent = a.nBlockDmgPercent + b.nBlockDmgPercent,
			nBlockPercent = a.nBlockPercent + b.nBlockPercent,
			nReduceBlockPercent = a.nReduceBlockPercent + b.nReduceBlockPercent
		};
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("op_Addition", typeof(PlayerStatus), typeof(PlayerStatus))]
    static bool op_Addition(PlayerStatus a, PlayerStatus b, ref PlayerStatus __result) {
		__result = new PlayerStatus {
			nHP = a.nHP + b.nHP,
			nATK = a.nATK + b.nATK,
			nDEF = a.nDEF + b.nDEF,
			nCRI = a.nCRI + b.nCRI,
			nHIT = a.nHIT + b.nHIT,
			nLuck = a.nLuck + b.nLuck,
			nDOD = a.nDOD + b.nDOD,
			nLV = a.nLV,
			nCriDmgPercent = a.nCriDmgPercent + b.nCriDmgPercent,
			nReduceCriPercent = a.nReduceCriPercent + b.nReduceCriPercent,
			nBlockDmgPercent = a.nBlockDmgPercent + b.nBlockDmgPercent,
			nBlockPercent = a.nBlockPercent + b.nBlockPercent,
			nReduceBlockPercent = a.nReduceBlockPercent + b.nReduceBlockPercent
		};
        return false;
	}
}

[HarmonyPatch(typeof(BulletBase))]
class BulletBase_ {

    static void _notify(int id) {
        EventManager.StageEventCall stageEventCall2 = new EventManager.StageEventCall();
        stageEventCall2.nID = id;
        Singleton<GenericEventManager>.Instance.NotifyEvent<EventManager.StageEventCall>(EventManager.ID.STAGE_EVENT_CALL, stageEventCall2);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(BulletBase.CalclDmgOnly))]
    static bool CalclDmgOnly(SKILL_TABLE pData, StageObjBase tSOB, ref int __result, ref BulletBase __instance)
    {
        float fDmg = 0, fBaseAtk = 0, fBaseDmg = 0;
		
        int tSOBType, nCurrentWeapon = 0, nLastHitStatus = 1;        
        int n_EFFECT = pData.n_EFFECT;
        
		__instance.damageType = VisualDamage.DamageType.Normal;
        tSOBType = tSOB.GetSOBType();

        if (tSOB.GetCurrentWeapon() == 1) {
			nCurrentWeapon = 1;
			nLastHitStatus |= 64;
		}

        switch (n_EFFECT) {
		case 0:
			if (tSOB.IsUnBreakX() && pData.n_TARGET != 2 && pData.n_TARGET != 3) {
                nLastHitStatus = nLastHitStatus | 128;
                __result = 5;
                goto __label_ret;
			}

__label_4_ex:
            nLastHitStatus = 0;
__label_4:
		    __result = 4;
            goto __label_ret;

		case 26:
			goto __label_4_ex;
            
        // Healing
		case 2:
            fDmg = ((float)__instance.nAtk * pData.f_EFFECT_X + pData.f_EFFECT_Y + (float)tSOB.MaxHp * pData.f_EFFECT_Z) * 0.01f;
			fDmg += fDmg * ((float)__instance.refPBMShoter.sBuffStatus.nHealEnhance * 0.01f);
            nLastHitStatus = 4;
			__result = 3;
            goto __label_set;

        // Status
		case 3:
		case 5:
		case 6:
		case 14:
		case 16:
			nLastHitStatus = 16;
			__result = 1;
            goto __label_set;

        // ???
		case 10:
			goto __label_4_ex;

		case 4:
		case 7:
		case 8:
			goto __label_4;

        // Similar to 0, but no nLastHitStatus = 0 in the else block
		case 18:
		case 19:
		case 20:
			if (tSOB.IsUnBreakX() && pData.n_TARGET != 2 && pData.n_TARGET != 3) {
                nLastHitStatus = nLastHitStatus | 128;
                __result = 5;
                goto __label_ret;
			}
			goto __label_4;

        // ???
		case 24:
			if (tSOB.IsUnBreakX()) {
				goto __label_4;
			}

			if (tSOBType == 1 && StageUpdate.bIsHost) {
				OrangeCharacter orangeCharacter = tSOB as OrangeCharacter;
				if ((orangeCharacter.bNeedUpdateAlways || MonoBehaviourSingleton<OrangeBattleServerManager>.Instance.CheckPlayerPause(orangeCharacter.sNetSerialID)) && orangeCharacter != null && !orangeCharacter.IsDead() && !orangeCharacter.IsInvincible) {
					if (pData.f_EFFECT_X != 0f || orangeCharacter.selfBuffManager.CheckHasEffectByCONDITIONID((int)pData.f_EFFECT_X, 0)) {
						orangeCharacter.Controller.LogicPosition = new VInt3(__instance.refPBMShoter.SOB._transform.position);
						orangeCharacter._transform.position = orangeCharacter.Controller.LogicPosition.vec3;
					}
				}
			}
			goto __label_4;

        // Always hit
        case 12:
        case 30:
            goto __label_hit;


        // Ignore invincible
        case 13:
            goto __label_dmg;

		}

        // Hit check
        if (!(tSOB.GetDOD(nCurrentWeapon) - __instance.nHit < OrangeBattleUtility.Random(0, 10000)) || !(__instance.fMissFactor * 100f <= (float)OrangeBattleUtility.Random(0, 10000))) {
            __result = 1;
            nLastHitStatus = 16;
            goto __label_set;
        }

__label_hit:
        // Invinicibility checks
        if (tSOB.IsUnBreakX()) {
            nLastHitStatus = nLastHitStatus | 128;
            __result = 5;
            goto __label_set;
        }
        // Hack: "__instance.tHurtPassParam.LVMax < 9999" -> "tSOBType == 1"
        if (tSOB.IsUnBreak() || ((tSOBType == 1) && tSOB.IsInvincible)) {
            nLastHitStatus = nLastHitStatus | 32;
            __result = 0;
            goto __label_set;
        }

__label_dmg:
        {
            float f_EFFECT_X = pData.f_EFFECT_X, f_EFFECT_Y = pData.f_EFFECT_Y, f_EFFECT_Z = pData.f_EFFECT_Z;
            __result = 2;

            switch (n_EFFECT) {
            
            case 13:
                fDmg = (float)tSOB.selfBuffManager.sBuffStatus.nEnergyShield;
                goto __label_dmg_12;

            case 12:
__label_dmg_12:
                fDmg += (float)(tSOB.MaxHp);
                __instance.fLastDmgFactor = fDmg;
                fDmg *= f_EFFECT_X * 0.01f;
                fBaseDmg = (fBaseAtk = fDmg);

                if (f_EFFECT_Z != 0f) { _notify((int)f_EFFECT_Z); }
                goto __label_set;

            case 30:
                fDmg = (float)(tSOB.MaxHp);
                __instance.fLastDmgFactor = fDmg;
                fDmg *= f_EFFECT_X * 0.01f;
                fBaseDmg = (fBaseAtk = fDmg);
                goto __label_misc;
            }

            float fLastDmgFactor = (pData.s_FIELD == "null") ? 
                __instance.fDmgFactor - tSOB.selfBuffManager.sBuffStatus.fReduceDmgPercent :
                100f + __instance.refPBMShoter.sBuffStatus.fAtkDmgPercent - tSOB.selfBuffManager.sBuffStatus.fReduceDmgPercent
            ;
            __instance.fLastDmgFactor = fLastDmgFactor;

            fBaseAtk = ((float)__instance.nAtk - (float)tSOB.GetDEF(nCurrentWeapon)) * (fLastDmgFactor * 0.01f);
            fBaseDmg = fBaseAtk * ((f_EFFECT_X + (float)__instance.BulletLevel * f_EFFECT_Y) * 0.01f);
            fDmg = fBaseDmg;

            if (n_EFFECT == 1 && f_EFFECT_Z != 0f && __instance.bCanUseInEventBullet) { _notify((int)f_EFFECT_Z); }

__label_misc:
            // No below checks for 12 and 13
            if (fBaseAtk < 0f) { fBaseAtk = 0; }
            if (fBaseDmg < 0f) { fBaseDmg = 0; }
            if (fDmg < 0f) { fDmg = 0; }

            // Critical
            {
                int nCri = __instance.nCri;
                if (((float)nCri - (float)tSOB.GetReduceCriPercent(nCurrentWeapon)) * __instance.fCriFactor >= (float)OrangeBattleUtility.Random(0, 1000000)) {
                    float fCri;
                
                    nLastHitStatus |= 2;
                    __instance.damageType = VisualDamage.DamageType.Cri;

                    nCri = __instance.nCriDmgPercent;
                    if (__instance.refPBMShoter.SOB as OrangeCharacter != null) {
                        nCri += OrangeConst.PLAYER_CRIDMG_BASE;
                    }
                    nCri = ManagedSingleton<StageHelper>.Instance.StatusCorrection(nCri, StageHelper.STAGE_RULE_STATUS.CRIDMG);
                    fCri = (float)nCri * __instance.refPSShoter.GetRatioStatus(6, __instance.nWeaponCheck);
                    fCri += (float)__instance.refPSShoter.GetAddStatus(6, __instance.nWeaponCheck) - (float)tSOB.GetReduceCriDmgPercent(nCurrentWeapon);
                    __instance.nLastCriPercent = (int)fCri;

                    fDmg += fDmg * (fCri * 0.0001f);
                    fDmg *= __instance.fCriDmgFactor * 0.01f;
                }
            }

            // Blocking
            if (tSOB.GetBlock() - __instance.nReduceBlockPercent >= OrangeBattleUtility.Random(0, 10000)) {
                float fBlc =  (float)tSOB.GetBlockDmgPercent();
                nLastHitStatus |= 8;
                __instance.damageType = VisualDamage.DamageType.Reduce;
                __instance.fLastBlockFactor = fBlc;
                fDmg -= fDmg * (fBlc * 0.0001f);
                if (fDmg < 0) { fDmg = 0; }
            }

            // Buffs
            {
                float fLastBuffFactor;

                if (__instance.nThrough * 100 < pData.n_THROUGH) {
                    fLastBuffFactor = (float)pData.n_THROUGH_DAMAGE;
                    __instance.tHurtPassParam.BulletFlg |= BulletBase.BulletFlag.Through;
                }
                else {
                    fLastBuffFactor = 100f;
                    __instance.tHurtPassParam.BulletFlg ^= BulletBase.BulletFlag.Through;
                }
                fLastBuffFactor = fLastBuffFactor * (100f + __instance.GetSkillDmgBuff(pData, tSOB.selfBuffManager) - (float)__instance.GetSkillResistanceBuff(pData, tSOB, (int)fDmg));
                __instance.nLastBuffFactor = (int)fLastBuffFactor;

                fLastBuffFactor *= 0.0001f;
                fBaseAtk *= fLastBuffFactor;
                fBaseDmg *= fLastBuffFactor;
                fDmg *= fLastBuffFactor;
            }
        }

__label_set:
        float fMax = 2147483647f;
        if (fBaseAtk < 0f) { fBaseAtk = 0; }
        if (fBaseDmg < 0f) { fBaseDmg = 0; }
        if (fDmg < 0f) { fDmg = 0; }
        __instance.nDmg = (fMax > fDmg) ? (int)fDmg : 0x7fffffff;
		__instance.nBaseAtk = (fMax > fBaseAtk) ? (int)fBaseAtk : 0x7fffffff;
		__instance.nBaseDmg = (fMax > fBaseDmg) ? (int)fBaseDmg : 0x7fffffff;

__label_ret:
        __instance.nLastHitStatus = nLastHitStatus;
        return false;
    }
}

[BepInPlugin(OrangeMods.MyPluginInfo.PLUGIN_GUID, OrangeMods.MyPluginInfo.PLUGIN_NAME, OrangeMods.MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin {
    public static new ManualLogSource Log;

    public void _inject(Type cls, Type[] interfaces) {
        ClassInjector.RegisterTypeInIl2Cpp(
            cls, 
            new RegisterTypeOptions() {
                Interfaces = new Il2CppInterfaceCollection(interfaces),
            }
        );
    }

    public override void Load() {
        Plugin.Log = base.Log;

        // Plugin startup logic
        Log.LogInfo($"Plugin {OrangeMods.MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony.CreateAndPatchAll(typeof(PlayerStatus_));
        Harmony.CreateAndPatchAll(typeof(BulletBase_));
        Harmony.CreateAndPatchAll(typeof(CharacterControlFactory_));

        _inject(typeof(CH091_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH092_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH093_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH098_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH099_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH100_ShungokusatsuDummy), new Type[] {} );
        _inject(typeof(CH100_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH106_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH107_CharacterMaterial), new Type[] {} );
        _inject(typeof(CH107_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH129_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH130_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH140_Controller), new Type[] {typeof(ILogicUpdate)});
        _inject(typeof(CH141_Controller), new Type[] {typeof(ILogicUpdate)});
    }
}
