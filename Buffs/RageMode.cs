using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.World;

namespace CalamityMod.Buffs
{
	public class RageMode : ModBuff
	{
        public static string RevTip = "150% damage boost. Can be boosted by other items up to 210%.";
        public static string DeathTip = "300% damage boost. Can be boosted by other items up to 510%.";

        public override void SetDefaults()
		{
			DisplayName.SetDefault("Rage Mode");
			Description.SetDefault(RevTip);
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

        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            if (CalamityWorld.death)
                tip = DeathTip;
        }
    }
}
