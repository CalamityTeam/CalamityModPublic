using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class BlueSpeedCandle : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Limber");
			Description.SetDefault("The floating flame seems to uplift your very spirit");
			Main.buffNoTimeDisplay[Type] = true;
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).blueCandle = true;
		}
	}
}