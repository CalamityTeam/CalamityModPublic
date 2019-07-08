using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Plaguebringer
{
    public class ThePlaguebringer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pandemic");
            Tooltip.SetDefault("Fires plague seekers when enemies are near");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 100;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("ThePlaguebringer");
        }
    }
}