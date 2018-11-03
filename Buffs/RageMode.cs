using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class RageMode : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Rage Mode");
			Description.SetDefault("100% (400% in Death Mode) increased damage.  Can be boosted by other items up to 160% (640% in Death Mode).");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).rageMode = true;
		}
	}
}