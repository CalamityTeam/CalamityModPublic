using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.SunkenSea
{
	public class ShellfishEating : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Shellfish Claps");
			Description.SetDefault("Clamfest");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).shellfishVore = true;
		}
	}
}