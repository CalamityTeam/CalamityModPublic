using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Accessories
{
    public class ArchaicPowder : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Archaic Powder");
            Tooltip.SetDefault("20% increased mining speed, 7% damage reduction, and +3 defense while underground or in the underworld");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight)
            {
                player.statDefense += 3;
                player.endurance += 0.07f;
                player.pickSpeed -= 0.2f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "AncientFossil");
            recipe.AddIngredient(null, "DemonicBoneAsh");
            recipe.AddIngredient(null, "AncientBoneDust", 3);
            recipe.AddIngredient(ItemID.Bone, 15);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}