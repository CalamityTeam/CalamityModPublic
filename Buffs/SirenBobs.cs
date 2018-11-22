using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class SirenBobs : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Siren");
            Description.SetDefault("You are a siren now");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>();
            if (modPlayer.sirenBoobsPrevious)
            {
                modPlayer.sirenBoobsPower = true;
                player.statDefense += 1 + 
                    (NPC.downedBoss3 ? 4 : 0) +
                    (Main.hardMode ? 10 : 0) +
                    (NPC.downedMoonlord ? 15 : 0);
                player.detectCreature = true;
                player.lifeRegen += 0 + 
                    (NPC.downedBoss3 ? 1 : 0) +
                    (Main.hardMode ? 1 : 0) +
                    (NPC.downedMoonlord ? 1 : 0);
                player.ignoreWater = NPC.downedBoss3;
                player.accFlipper = true;
                player.discount = Main.hardMode;
                if (player.breath <= player.breathMax + 2 && !modPlayer.ZoneAbyss && NPC.downedBoss3)
                {
                    player.breath = player.breathMax + 3;
                }
                if (Main.myPlayer == player.whoAmI && player.wet && NPC.downedBoss3)
                {
                    player.AddBuff(mod.BuffType("SirenWaterSpeed"), 360);
                }
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
	}
}