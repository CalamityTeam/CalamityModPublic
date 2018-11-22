using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class ScarfCooldown : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Scarf Cooldown");
			Description.SetDefault("Your dodge is recharging");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).scarfCooldown = true;
		}
	}
}