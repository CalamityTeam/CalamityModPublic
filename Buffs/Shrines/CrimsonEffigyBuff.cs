using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Shrines
{
	public class CrimsonEffigyBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Crimson Effigy");
			Description.SetDefault("The crimson empowers you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).crimEffigy = true;
		}
	}
}