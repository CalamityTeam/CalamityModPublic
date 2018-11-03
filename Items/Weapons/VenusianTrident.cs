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
    public class VenusianTrident : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Venusian Trident");
            Tooltip.SetDefault("Casts an inferno bolt that erupts into a gigantic explosion of fire and magma shards");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 360;
            item.magic = true;
            item.mana = 20;
            item.width = 48;
            item.height = 48;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 9f;
            item.value = 800000;
            item.UseSound = SoundID.Item45;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("VenusianBolt");
            item.shootSpeed = 19f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.InfernoFork);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "TwistingNether");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}