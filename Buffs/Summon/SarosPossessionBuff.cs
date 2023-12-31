﻿using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class SarosPossessionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SarosAura>()] > 0)
                modPlayer.saros = true;
            if (!modPlayer.saros)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
                player.buffTime[buffIndex] = 18000;
        }
    }
}
