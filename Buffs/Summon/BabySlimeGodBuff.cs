﻿using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class BabySlimeGodBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<CrimsonSlimeGodMinion>()] > 0)
            {
                modPlayer.sGod = true;
            }
            else if (player.ownedProjectileCounts[ModContent.ProjectileType<CorruptionSlimeGodMinion>()] > 0)
            {
                modPlayer.sGod = true;
            }
            if (!modPlayer.sGod)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
