using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Items.Accessories
{
    public class NecklaceofVexation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Necklace of Vexation");
            Tooltip.SetDefault("Revenge\n" +
            "15% increased damage when under 50% life");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.rare = 6;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife < (player.statLifeMax2 * 0.5f))
            {
                player.meleeDamage += 0.15f;
                player.magicDamage += 0.15f;
                player.rangedDamage += 0.15f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
                player.minionDamage += 0.15f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 2);
            recipe.AddIngredient(ItemID.AvengerEmblem);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}