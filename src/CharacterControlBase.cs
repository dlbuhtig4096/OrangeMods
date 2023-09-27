
using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CharacterControlBase_ : CharacterControlBase {
    public CharacterControlBase_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<CharacterControlBase_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
        rtld.CharacterControlBase_ctor(this.Pointer);
    }
    
    public CharacterControlBase_(IntPtr p) : base(p) {
        rtld.CharacterControlBase_ctor(p);
    }

    public override void Start() {
        this.CallBase<CharacterControlBase>("Start"); // base.Start();
    }

    public override void CheckSkill() {}
    public override void ClearSkill() {}
    public override void PlayerPressSkillCharacterCall(int id) {}
    public override void PlayerReleaseSkillCharacterCall(int id) {}
}
