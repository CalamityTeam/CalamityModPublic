using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.World;

namespace CalamityMod.Buffs
{
	public class AdrenalineMode : ModBuff
	{
		public static string RevTip = "250% damage boost. Can burnout down to 149.5%.";
		public static string DeathTip = "700% damage boost. Can burnout down to 298%.";

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Adrenaline Mode");
			Description.SetDefault(RevTip);
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).adrenalineMode = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			if (CalamityWorld.death)
				tip = DeathTip;
		}
	}
}
