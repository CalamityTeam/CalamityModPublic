using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ArmorCrunch : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Armor Crunch");
			Description.SetDefault("Your armor is shredded");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}
		
		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).aCrunch = true;
		}
	}
}