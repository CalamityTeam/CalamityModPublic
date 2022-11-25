﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBrimstoneCrag : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Crag");
            Tooltip.SetDefault("Ah... this place.\n" +
                "The scent of broken promises, pain, and eventual death is heavy in the air...");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneElementalTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
