using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class AdrenalineMode : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Adrenaline Mode");
			Description.SetDefault("250% (1000% in Death Mode) increased damage.");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).adrenalineMode = true;
		}
	}
}