using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.SunkenSea
{
	public class PearlAura : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Pearl Aura");
			Description.SetDefault("Slowed down");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).pearlAura = true;
		}
	}
}