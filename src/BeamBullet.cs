using System;
using System.Collections;
using System.Collections.Generic;
using CallbackDefs;
using StageLib;
using UnityEngine;

public abstract class BeamBullet_ : BeamBullet {

    public BeamBullet_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<BeamBullet_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
    }
    
    public BeamBullet_(IntPtr p) : base(p) {
        // rtld.BeamBullet_ctor(p);
        this.CallBase<BeamBullet>(".ctor");
    }

}
