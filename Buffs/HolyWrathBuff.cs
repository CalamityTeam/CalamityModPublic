using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class HolyWrathBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Holy Wrath");
			Description.SetDefault("Increased damage, increased movement and flight speed, and all attacks inflict holy fire");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).holyWrath = true;
		}
	}
}
