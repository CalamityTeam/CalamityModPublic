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
    public class DeificAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deific Amulet");
            Tooltip.SetDefault("Taking damage makes you move very fast for a short time\n" +
                               "Increases armor penetration by 25 and immune time after being struck\n" +
                               "Provides light underwater and causes stars to fall when damaged\n" +
                               "Increases pickup range for mana stars and you restore mana when damaged");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 8;
            item.value = 1500000;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.dAmulet = true;
            player.panic = true;
            player.armorPenetration += 25;
            player.manaMagnet = true;
            player.magicCuffs = true;
            if (player.wet)
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 1.35f, 0.3f, 0.9f);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CelestialCuffs);
            recipe.AddIngredient(ItemID.JellyfishNecklace);
            recipe.AddIngredient(ItemID.PanicNecklace);
            recipe.AddIngredient(ItemID.SharkToothNecklace);
            recipe.AddIngredient(ItemID.StarVeil);
            recipe.AddIngredient(null, "Stardust", 25);
            recipe.AddIngredient(ItemID.MeteoriteBar, 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}