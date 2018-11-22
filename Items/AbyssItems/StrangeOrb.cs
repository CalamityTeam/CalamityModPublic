using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.AbyssItems
{
    public class StrangeOrb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Orb");
            Tooltip.SetDefault("Summons a young Siren light pet\n" +
                "Provides a large amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.WispinaBottle);
            item.shoot = mod.ProjectileType("SirenYoung");
            item.buffType = mod.BuffType("StrangeOrb");
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}