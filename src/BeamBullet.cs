using System;
using System.Collections;
using System.Collections.Generic;
using CallbackDefs;
using StageLib;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public abstract class BeamBullet_ : BeamBullet {

    public BeamBullet_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<BeamBullet_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
    }
    
    public BeamBullet_(IntPtr p) : base(p) {
        this.bulletFxArray = new ParticleSystem[0];
    }

}
