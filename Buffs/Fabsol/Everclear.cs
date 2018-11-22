using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class Everclear : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Everclear");
			Description.SetDefault("Damage and critical strike chance boosted, life regen and defense reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).everclear = true;
		}
	}
}