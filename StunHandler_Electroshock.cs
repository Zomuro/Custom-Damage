using RimWorld;
using Verse;
using System;
using UnityEngine;

namespace Electroshock
{
    public class StunHandler_Electroshock : IExposable
	{
		// Token: 0x17000E5C RID: 3676
		// (get) Token: 0x060051D7 RID: 20951 RVA: 0x001B93CA File Offset: 0x001B75CA
		public bool Stunned
		{
			get
			{
				return this.stunTicksLeft > 0;
			}
		}

		// Token: 0x17000E5D RID: 3677
		// (get) Token: 0x060051D8 RID: 20952 RVA: 0x001B93D8 File Offset: 0x001B75D8
		private int EMPAdaptationTicksDuration
		{
			get
			{
				Pawn pawn = this.parent as Pawn;
				if (pawn != null && pawn.RaceProps.IsMechanoid)
				{
					return 2200;
				}
				return 0;
			}
		}

		// Token: 0x17000E5E RID: 3678
		// (get) Token: 0x060051D9 RID: 20953 RVA: 0x001B9408 File Offset: 0x001B7608
		private bool AffectedByEMP
		{
			get
			{
				Pawn pawn;
				return (pawn = (this.parent as Pawn)) == null || !pawn.RaceProps.IsFlesh;
			}
		}

		// Token: 0x17000E5F RID: 3679
		// (get) Token: 0x060051DA RID: 20954 RVA: 0x001B9434 File Offset: 0x001B7634
		public int StunTicksLeft
		{
			get
			{
				return this.stunTicksLeft;
			}
		}

		// Token: 0x060051DB RID: 20955 RVA: 0x001B943C File Offset: 0x001B763C
		public StunHandler_Electroshock(Thing parent)
		{
			this.parent = parent;
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x001B9454 File Offset: 0x001B7654
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.stunTicksLeft, "stunTicksLeft", 0, false);
			Scribe_Values.Look<bool>(ref this.showStunMote, "showStunMote", false, false);
			Scribe_Values.Look<int>(ref this.EMPAdaptedTicksLeft, "EMPAdaptedTicksLeft", 0, false);
			Scribe_Values.Look<bool>(ref this.stunFromEMP, "stunFromEMP", false, false);
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x001B94AC File Offset: 0x001B76AC
		public void StunHandlerTick()
		{
			if (this.EMPAdaptedTicksLeft > 0)
			{
				this.EMPAdaptedTicksLeft--;
			}
			if (this.stunTicksLeft > 0)
			{
				this.stunTicksLeft--;
				if (this.showStunMote && (this.moteStun == null || this.moteStun.Destroyed))
				{
					this.moteStun = MoteMaker.MakeStunOverlay(this.parent);
				}
				Pawn pawn = this.parent as Pawn;
				if (pawn != null && pawn.Downed)
				{
					this.stunTicksLeft = 0;
				}
				if (this.moteStun != null)
				{
					this.moteStun.Maintain();
				}
				if (this.AffectedByEMP && this.stunFromEMP)
				{
					if (this.empEffecter == null)
					{
						this.empEffecter = EffecterDefOf.DisabledByEMP.Spawn();
					}
					this.empEffecter.EffectTick(this.parent, this.parent);
					return;
				}
			}
			else if (this.empEffecter != null)
			{
				this.empEffecter.Cleanup();
				this.empEffecter = null;
				this.stunFromEMP = false;
			}
		}

		// Token: 0x060051DE RID: 20958 RVA: 0x001B95B4 File Offset: 0x001B77B4
		public void Notify_DamageApplied(DamageInfo dinfo, bool affectedByEMP)
		{
			Pawn pawn = this.parent as Pawn;
			if (pawn != null && (pawn.Downed || pawn.Dead))
			{
				return;
			}
			if (dinfo.Def == DamageDefOf_Electroshock.Electroshock && this.AffectedByEMP == false)
			{
				this.StunFor_NewTmp(Mathf.RoundToInt(dinfo.Amount * 30f), dinfo.Instigator, true, true);
				return;
			}
			if (dinfo.Def == DamageDefOf_Electroshock.Electroshock && this.AffectedByEMP)
			{
				if (this.EMPAdaptedTicksLeft <= 0)
				{
					this.StunFor_NewTmp(Mathf.RoundToInt(dinfo.Amount * 30f), dinfo.Instigator, true, true);
					this.EMPAdaptedTicksLeft = this.EMPAdaptationTicksDuration;
					this.stunFromEMP = true;
					return;
				}
				MoteMaker.ThrowText(new Vector3((float)this.parent.Position.x + 1f, (float)this.parent.Position.y, (float)this.parent.Position.z + 1f), this.parent.Map, "Adapted".Translate(), Color.white, -1f);
			}
		}

		// Token: 0x060051DF RID: 20959 RVA: 0x001B96D8 File Offset: 0x001B78D8
		[Obsolete("Only need this overload to not break mod compatibility.")]
		public void StunFor(int ticks, Thing instigator, bool addBattleLog = true)
		{
			this.StunFor_NewTmp(ticks, instigator, addBattleLog, true);
		}

		// Token: 0x060051E0 RID: 20960 RVA: 0x001B96E4 File Offset: 0x001B78E4
		public void StunFor_NewTmp(int ticks, Thing instigator, bool addBattleLog = true, bool showMote = true)
		{
			this.stunTicksLeft = Mathf.Max(this.stunTicksLeft, ticks);
			this.showStunMote = showMote;
			if (addBattleLog)
			{
				Find.BattleLog.Add(new BattleLogEntry_Event(this.parent, RulePackDefOf.Event_Stun, instigator));
			}
		}

		// Token: 0x04002E42 RID: 11842
		public Thing parent;

		// Token: 0x04002E43 RID: 11843
		private int stunTicksLeft;

		// Token: 0x04002E44 RID: 11844
		private Mote moteStun;

		// Token: 0x04002E45 RID: 11845
		private bool showStunMote = true;

		// Token: 0x04002E46 RID: 11846
		private int EMPAdaptedTicksLeft;

		// Token: 0x04002E47 RID: 11847
		private Effecter empEffecter;

		// Token: 0x04002E48 RID: 11848
		private bool stunFromEMP;

		// Token: 0x04002E49 RID: 11849
		public const float StunDurationTicksPerDamage = 30f;
	}
}
