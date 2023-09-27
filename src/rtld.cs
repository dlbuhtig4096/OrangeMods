using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

static class rtld {

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    public delegate void ctor(IntPtr self);
    public delegate void cctor();
    public static ctor CharacterControlBase_ctor;
    public static ctor CharacterControllerProxyBaseGen3_ctor;
    public static ctor CollideBullet_ctor;
    public static ctor BeamBullet_ctor;
    public static ctor CharacterMaterial_ctor;

    static rtld() {
        IntPtr p = GetModuleHandle("GameAssembly.dll");
        
        CharacterControlBase_ctor = Marshal.GetDelegateForFunctionPointer<ctor>(p + 0x343100);
        CharacterControllerProxyBaseGen3_ctor = Marshal.GetDelegateForFunctionPointer<ctor>(p + 0x5998A0);
        CollideBullet_ctor = Marshal.GetDelegateForFunctionPointer<ctor>(p + 0x9B50B0);
        BeamBullet_ctor = Marshal.GetDelegateForFunctionPointer<ctor>(p + 0x9D5E70);
        CharacterMaterial_ctor = Marshal.GetDelegateForFunctionPointer<ctor>(p + 0xD60260);

        // BulletBase.cctor
        // Marshal.GetDelegateForFunctionPointer<cctor>(p + 0x7F4180)();
    }
}

/*
var fi = typeof(CollideBullet).GetFields();
Plugin.Log.LogWarning(fi.Length);
for(int i = 0; i < fi.Length; i++) {
    Plugin.Log.LogWarning($"Name            : {fi[i].Name}");
    Plugin.Log.LogWarning($"Declaring Type  : {fi[i].DeclaringType}");
    Plugin.Log.LogWarning($"IsPublic        : {fi[i].IsPublic}");
    Plugin.Log.LogWarning($"MemberType      : {fi[i].MemberType}");
    Plugin.Log.LogWarning($"FieldType       : {fi[i].FieldType}");
    Plugin.Log.LogWarning($"IsFamily        : {fi[i].IsFamily}");
}
*/