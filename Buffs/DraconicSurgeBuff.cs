using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class DraconicSurgeBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Draconic Surge");
			Description.SetDefault("The power of a dragon courses through your veins");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
		}
		
		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).draconicSurge = true;
		}
	}
}
