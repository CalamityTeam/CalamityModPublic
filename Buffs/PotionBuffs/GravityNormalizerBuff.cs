﻿using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class GravityNormalizerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Gravity Normalizer");
            Description.SetDefault("Gravity is now normal in space, immunity to distorted");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().gravityNormalizer = true;
        }
    }
}
