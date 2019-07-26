using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class MoscowMule : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Moscow Mule");
			Description.SetDefault("Damage, critical strike chance, and knockback boosted, life regen reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).moscowMule = true;
		}
	}
}
