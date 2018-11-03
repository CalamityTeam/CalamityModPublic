using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class IceShieldBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Ice Shield");
			Description.SetDefault("The shield will absorb 50% damage from one hit before breaking");
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).sirenIce = true;
		}
	}
}