
using System;
using CallbackDefs;
using UnityEngine;
using StageLib;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public static class FxManager_ {

    public static void Play(string p_fxName, Vector3 p_worldPos, Quaternion p_quaternion, Il2CppReferenceArray<Il2CppSystem.Object> p_params = null) {
        if (MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(p_fxName)) {
            FxBase poolObj = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<FxBase>(p_fxName);
            poolObj.transform.SetParent(null);
            poolObj.transform.SetPositionAndRotation(p_worldPos, p_quaternion);
            FxManager.Instance.RegisterFxBase(poolObj);
            poolObj.Active(p_params);
            if (p_params != null && p_params.Length != 0) {
                StageFXParam stageFXParam = p_params[0] as StageFXParam;
                if (stageFXParam != null) {
                    if (stageFXParam.tFOL != null) {
                        stageFXParam.tFOL.tObj = poolObj.gameObject;
                    }
                    FxManager.Instance.ChangeFXColor(poolObj, stageFXParam);
                    return;
                }
            }
        }
        else {
            FxManager.Instance.PreloadFx(p_fxName, 1, (Callback)(new Action(() => Play(p_fxName, p_worldPos, p_quaternion, p_params))));
        }
    }

    public static void Play(string pFxName, Transform pTransform, Quaternion pQuaternion, Il2CppReferenceArray<Il2CppSystem.Object> pParams = null) {
        if (MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(pFxName)) {
            FxBase poolObj = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<FxBase>(pFxName);
            Vector3 localScale = poolObj.transform.localScale;
            poolObj.transform.SetParent(pTransform);
            poolObj.transform.localPosition = Vector3.zero;
            poolObj.transform.localRotation = pQuaternion;
            poolObj.transform.localScale = localScale;
            FxManager.Instance.RegisterFxBase(poolObj);
            poolObj.Active(pParams);
            if (pParams != null && pParams.Length != 0) {
                StageFXParam stageFXParam = pParams[0] as StageFXParam;
                if (stageFXParam != null) {
                    if (stageFXParam.tFOL != null) {
                        stageFXParam.tFOL.tObj = poolObj.gameObject;
                    }
                    FxManager.Instance.ChangeFXColor(poolObj, stageFXParam);
                    return;
                }
            }
        }
        else {
            FxManager.Instance.PreloadFx(pFxName, 1, (Callback)(new Action(() => Play(pFxName, pTransform, pQuaternion, pParams))));
        }
    }

    public static void Play(string pFxName, Transform pTransform, Quaternion pQuaternion, Vector3 pScale, Il2CppReferenceArray<Il2CppSystem.Object> pParams = null) {
        if (MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(pFxName)) {
            FxBase poolObj = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<FxBase>(pFxName);
            poolObj.transform.SetParent(pTransform);
            poolObj.transform.localPosition = Vector3.zero;
            poolObj.transform.localRotation = pQuaternion;
            poolObj.transform.localScale = pScale;
            FxManager.Instance.RegisterFxBase(poolObj);
            poolObj.Active(pParams);
            if (pParams != null && pParams.Length != 0) {
                StageFXParam stageFXParam = pParams[0] as StageFXParam;
                if (stageFXParam != null) {
                    if (stageFXParam.tFOL != null) {
                        stageFXParam.tFOL.tObj = poolObj.gameObject;
                    }
                    FxManager.Instance.ChangeFXColor(poolObj, stageFXParam);
                    return;
                }
            }
        }
        else {
            FxManager.Instance.PreloadFx(pFxName, 1, (Callback)(new Action(() => Play(pFxName, pTransform, pQuaternion, pScale, pParams))));
        }
    }

    public static T PlayReturn<T>(string pFxName, Transform pTransform, Quaternion pQuaternion, Il2CppReferenceArray<Il2CppSystem.Object> pParams = null) where T : FxBase {
        if (MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(pFxName)) {
            T poolObj = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<T>(pFxName);
            Vector3 localScale = poolObj.transform.localScale;
            poolObj.transform.SetParent(pTransform);
            poolObj.transform.localPosition = Vector3.zero;
            poolObj.transform.localRotation = pQuaternion;
            poolObj.transform.localScale = localScale;
            FxManager.Instance.RegisterFxBase(poolObj);
            poolObj.Active(pParams);
            return poolObj;
        }
        return default(T);
    }

    public static T PlayReturn<T>(string pFxName, Vector3 p_worldPos, Quaternion pQuaternion, Il2CppReferenceArray<Il2CppSystem.Object> pParams = null) where T : FxBase {
        if (MonoBehaviourSingleton<PoolManager>.Instance.IsPreload(pFxName)) {
            T poolObj = MonoBehaviourSingleton<PoolManager>.Instance.GetPoolObj<T>(pFxName);
            poolObj.transform.SetParent(null);
            poolObj.transform.SetPositionAndRotation(p_worldPos, pQuaternion);
            FxManager.Instance.RegisterFxBase(poolObj);
            poolObj.Active(pParams);
            return poolObj;
        }
        return default(T);
    }

    /*
    public static void UpdateFx(ParticleSystem[] p_fxArray, bool p_active, ParticleSystemStopBehavior particleSystemStopBehavior = ParticleSystemStopBehavior.StopEmitting) {
        
        if (p_active) {
            for (int i = 0; i < p_fxArray.Length; i++) {
                // Plugin.Log.LogWarning($"{i} / {p_fxArray.Length}: {p_fxArray[i]}");
                ParticleSystem particleSystem = p_fxArray[i];
                FxManager.Instance.RegisterFxBase(particleSystem);
                particleSystem.Simulate(0f, false, true); // Access Violation
                particleSystem.Play(false);
            }
            return;
        }
        for (int i = 0; i < p_fxArray.Length; i++) {
            // Plugin.Log.LogWarning($"{i} / {p_fxArray.Length}: {p_fxArray[i]}");
            ParticleSystem particleSystem = p_fxArray[i];
            FxManager.Instance.UnRegisterFxBase(particleSystem);
            if (FxManager.Instance.bLastLock) {
                particleSystem.Play(false);
            }
            particleSystem.Stop(false, particleSystemStopBehavior);
        }
    }
    */
}