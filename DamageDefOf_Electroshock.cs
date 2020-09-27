using Verse;
using RimWorld;

namespace Electroshock
{
    [DefOf]
    public static class DamageDefOf_Electroshock
    {
        public static DamageDef Electroshock;
        static DamageDefOf_Electroshock() //connects xml code to C# code
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(DamageDefOf_Electroshock));
        }
    }
}
