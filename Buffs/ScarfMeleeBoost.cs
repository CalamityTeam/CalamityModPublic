using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ScarfMeleeBoost : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Scarf Boost");
			Description.SetDefault("10% increased damage, 5% increased crit chance, and 10% increased melee speed");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).sMeleeBoost = true;
		}
	}
}