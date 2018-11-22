using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons
{
    public class Zapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lazinator");
            Tooltip.SetDefault("Zap");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.magic = true;
            item.mana = 4;
            item.width = 46;
            item.height = 22;
            item.useTime = 7;
            item.useAnimation = 7;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = 180000;
            item.rare = 5;
            item.UseSound = SoundID.Item12;
            item.autoReuse = true;
            item.shoot = 88;
            item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(null, "VictoryShard", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}