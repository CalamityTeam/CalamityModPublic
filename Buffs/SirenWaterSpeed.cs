using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class SirenWaterSpeed : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Siren Speed");
            Description.SetDefault("15% increased max speed and acceleration");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.GetModPlayer<CalamityPlayer>(mod).sirenWaterBuff = true;
        }
	}
}