
/*
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public static class gch {

    public static Dictionary<IntPtr, GCHandle[]> m;

    static gch() {
        m = new ();
    }
    
    public static void alloc(IntPtr p, object[] a) {
        Plugin.Log.LogDebug($"Alloc: {p}");
        if (m.ContainsKey(p)) { return; }
        m[p] = Array.ConvertAll(a, o => GCHandle.Alloc(o));
        foreach (object o in a) { Plugin.Log.LogDebug($"{o}"); }
    }

    public static void free(IntPtr p) {
        Plugin.Log.LogDebug($"Free: {p}");
        GCHandle[] a;
        if (!m.Remove(p, out a)) { return; }
        foreach (GCHandle o in a) { o.Free(); }
    }
}
*/

