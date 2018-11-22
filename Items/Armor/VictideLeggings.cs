using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class VictideLeggings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Leggings");
            Tooltip.SetDefault("Movement speed increased by 8%\n" +
                "Speed greatly increased while submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = 37500;
            item.rare = 2;
            item.defense = 4; //9
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.08f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.moveSpeed += 0.5f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 4);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}