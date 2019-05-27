using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ProfanedRageBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Profaned Rage");
			Description.SetDefault("Increased crit chance, increased movement and flight speed, and you gain more rage when damaged");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).profanedRage = true;
		}
	}
}