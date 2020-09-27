using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace Electroshock
{
	// Token: 0x02000230 RID: 560
	public class DamageWorker_Electroshock : DamageWorker
	{
		// Token: 0x06000FA6 RID: 4006 RVA: 0x0005AA39 File Offset: 0x00058C39
		public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing victim)
		{
			DamageWorker.DamageResult damageResult = base.Apply(dinfo, victim);
			damageResult.stunned = true;
			return damageResult;
		}
		public override void ExplosionAffectCell(Explosion explosion, IntVec3 c, List<Thing> damagedThings, List<Thing> ignoredThings, bool canThrowMotes)
		{
			base.ExplosionAffectCell(explosion, c, damagedThings, ignoredThings, canThrowMotes);
		}
	}
}
