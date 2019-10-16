﻿using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    class OnyxExcavatorBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Onyx Excavator");
            Description.SetDefault("Drill");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<OnyxExcavator>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().onyxExcavator = true;
        }
    }
}
