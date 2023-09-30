using System;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

public class CharacterMaterial_ : CharacterMaterial {
    public CharacterMaterial_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<CharacterMaterial_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
        // rtld.CharacterMaterial_ctor(this.Pointer);
    }
    
    public CharacterMaterial_(IntPtr p) : base(p) {
        // rtld.CharacterMaterial_ctor(p);
        this.CallBase<CharacterMaterial>(".ctor");
    }
}
