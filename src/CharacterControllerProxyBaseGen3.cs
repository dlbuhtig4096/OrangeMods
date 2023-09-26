using System;
using System.Collections.Generic;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public abstract class CharacterControllerProxyBaseGen3_ : CharacterControllerProxyBaseGen3 {

    public CharacterControllerProxyBaseGen3_(): base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<CharacterControllerProxyBaseGen3_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
    }

    public CharacterControllerProxyBaseGen3_(IntPtr p): base(p) {
        // Hack: for some reason dictionaries cannot not be properly initialized in .ctor, so we do it here.
        this.OnPlayerPressSkill0Events = new Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Action<CharacterControllerProxyBaseGen1.SkillID>>();
        this.OnPlayerPressSkill1Events = new Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Action<CharacterControllerProxyBaseGen1.SkillID>>();
        this.OnPlayerReleaseSkill0Events = new Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Action<CharacterControllerProxyBaseGen1.SkillID>>();
        this.OnPlayerReleaseSkill1Events = new Il2CppSystem.Collections.Generic.Dictionary<int, Il2CppSystem.Action<CharacterControllerProxyBaseGen1.SkillID>>();
    }

    public override void Start() {
        this.CallBase<CharacterControllerProxyBaseGen3>("Start"); // base.Start();
    }
    
}
