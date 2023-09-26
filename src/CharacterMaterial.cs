using System;
using MagicaCloth;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public abstract class CharacterMaterial_ : CharacterMaterial {

    public Color RimColor { get; set; } = new Color(1f, 1f, 1f, 0.15f);

    public CharacterMaterial_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<CharacterMaterial_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
    }
    
    public CharacterMaterial_(IntPtr p) : base(p) {
        // this.renderers = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<UnityEngine.Renderer>(base.GetComponentsInChildren<Renderer>());
        this.OutlineColor = new Color(0.142f, 0.168f, 0.227f);
    }
}
