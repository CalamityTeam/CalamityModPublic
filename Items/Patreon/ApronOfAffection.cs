using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Patreon;

namespace CalamityMod.Items.Patreon
{
    [AutoloadEquip(EquipType.Body)]
    public class ApronOfAffection : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ace's Apron of Affection");
            Tooltip.SetDefault("Great for hugging people");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.rare = 5;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
			item.value = Item.buyPrice(0, 15, 0, 0);
            item.vanity = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Robe);
            recipe.AddIngredient(ItemID.LovePotion, 10);
            recipe.AddIngredient(ItemID.LifeCrystal);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
