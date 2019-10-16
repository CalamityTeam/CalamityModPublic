﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    class OnyxExcavatorKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Excavator Key");
            Tooltip.SetDefault("Summons a drill to drill through the world so you can destroy all the neat world generation\n" +
                "with complete disregard for all the creatures that inhabit these lands. I am sure the EPA and PETA would like\n" +
                "to have a word with you afterwards.");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 4;
            item.rare = 3;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.UseSound = SoundID.Item23;
            item.noMelee = true;
            item.mountType = ModContent.MountType<OnyxExcavator>();
        }
    }
}
