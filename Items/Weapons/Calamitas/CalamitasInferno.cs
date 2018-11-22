using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Calamitas
{
    public class CalamitasInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lashes of Chaos");
            Tooltip.SetDefault("Watch the world burn...");
        }

        public override void SetDefaults()
        {
            item.damage = 98;
            item.magic = true;
            item.mana = 20;
            item.width = 28;
            item.height = 30;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true; //so the item's animation doesn't do damage
            item.knockBack = 7.5f;
            item.value = 550000;
            item.rare = 7;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("BrimstoneHellfireballFriendly");
            item.shootSpeed = 16f;
        }
    }
}