using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Accessories
{
    public class GoldBurdenBreaker : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burden Breaker");
            Tooltip.SetDefault("The good time\nGo fast\nWARNING: May have disastrous results");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = 150000;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (CalamityGlobalNPC.AnyBossNPCS()) { return; }
            if (player.velocity.X > 5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X >= 500f)
                {
                    player.velocity.X = 0f;
                }
            }
            else if (player.velocity.X < -5f)
            {
                player.velocity.X *= 1.025f;
                if (player.velocity.X <= -500f)
                {
                    player.velocity.X = 0f;
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.GoldBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(ItemID.PlatinumBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}