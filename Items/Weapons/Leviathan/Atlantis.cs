using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.Leviathan
{
    public class Atlantis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlantis");
            Tooltip.SetDefault("Casts aquatic spears");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.magic = true;
            item.mana = 12;
            item.width = 28;
            item.height = 30;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = 600000;
            item.rare = 7;
            item.UseSound = SoundID.Item34;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("AtlantisSpear");
            item.shootSpeed = 32f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "IOU");
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}