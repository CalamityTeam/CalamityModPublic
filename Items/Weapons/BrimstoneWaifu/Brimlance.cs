using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.BrimstoneWaifu
{
    public class Brimlance : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimlance");
            Tooltip.SetDefault("This spear causes brimstone explosions on enemy hits\n" +
				"Enemies killed by the spear drop brimstone fire");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.damage = 75;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 19;
            item.useStyle = 5;
            item.useTime = 19;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = false;
            item.height = 56;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.shoot = mod.ProjectileType("Brimlance");
            item.shootSpeed = 12f;
        }
    }
}
