using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Fabsol
{
	public class BloodyMary : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Bloody Mary");
			Description.SetDefault("Damage, critical strike chance, movement speed, and melee speed boosted during a Blood Moon, life regen and defense reduced");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).bloodyMary = true;
		}
	}
}
