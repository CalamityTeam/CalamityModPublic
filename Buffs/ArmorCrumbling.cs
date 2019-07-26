using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ArmorCrumbling : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Armor Crumbling");
			Description.SetDefault("Melee and rogue attacks break enemy armor");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).armorCrumbling = true;
		}
	}
}
