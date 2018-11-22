using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class EvergreenGin : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Evergreen Gin");
			Description.SetDefault("Nature-based weapon damage and damage reduction boosted, life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).evergreenGin = true;
		}
	}
}