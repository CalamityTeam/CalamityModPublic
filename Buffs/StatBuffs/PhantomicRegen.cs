﻿using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class PhantomicRegen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer calPlayer = player.Calamity();
            if (calPlayer.phantomicHeartRegen <= 0)
            {
                calPlayer.phantomicHeartRegen = 1000;
            }
            player.lifeRegen += 2;
        }
    }
}
