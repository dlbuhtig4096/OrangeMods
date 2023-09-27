using System;
using CallbackDefs;
using UnityEngine;

using Il2CppInterop.Runtime.InteropTypes.Arrays;

public abstract class CollideBullet_ : CollideBullet {
    
    public CollideBullet_() : base(Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorPointer<CollideBullet_>()) {
        Il2CppInterop.Runtime.Injection.ClassInjector.DerivedConstructorBody(this);
        rtld.CollideBullet_ctor(this.Pointer);
        
        // New field introduced in offline
        this.listHit = new Il2CppSystem.Collections.Generic.List<Collider2D>(); // 0x2F0
        this.listHitID = new Il2CppSystem.Collections.Generic.List<int>(); // 0x2F8
        this.listRemoveHit = new Il2CppSystem.Collections.Generic.List<Collider2D>(); // 0x300
    }
    
    public CollideBullet_(IntPtr p) : base(p) {
        rtld.CollideBullet_ctor(p);
        // Hack: initialize objects manually since base(p) is not working for some reasons
        /*
        this._hitCount = new Il2CppSystem.Collections.Generic.Dictionary<Transform, int>();
        this.bulletFxArray = new ParticleSystem[0];
        this.tHurtPassParam = new HurtPassParam();

        */
        // New field introduced in offline
        this.listHit = new Il2CppSystem.Collections.Generic.List<Collider2D>(); // 0x2F0
        this.listHitID = new Il2CppSystem.Collections.Generic.List<int>(); // 0x2F8
        this.listRemoveHit = new Il2CppSystem.Collections.Generic.List<Collider2D>(); // 0x300
    }

}
