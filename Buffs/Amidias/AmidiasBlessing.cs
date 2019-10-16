﻿using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class AmidiasBlessing : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Amidias' Blessing");
            Description.SetDefault("You are blessed by Amidias" +
                                   "\nLets you breathe underwater, even in the Abyss!" +
                                   "\nJust don't get hit...");
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().amidiasBlessing = true;
            player.breath = player.breathMax + 91;
        }
    }
}
