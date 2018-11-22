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
    public class FrigidBulwark : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frigid Bulwark");
            Tooltip.SetDefault("Absorbs 25% of damage done to players on your team\n" +
                "Only active above 25% life\n" +
                "Grants immunity to knockback\n" +
                "Puts a shell around the owner when below 50% life that reduces damage\n" +
                "The shell becomes more powerful when below 15% life and reduces damage even further");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = 5000000;
            item.rare = 9;
            item.defense = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.noKnockback = true;
            if ((float)player.statLife > (float)player.statLifeMax2 * 0.25f)
            {
                player.hasPaladinShield = true;
                if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0)
                {
                    int myPlayer = Main.myPlayer;
                    if (Main.player[myPlayer].team == player.team && player.team != 0)
                    {
                        float arg = player.position.X - Main.player[myPlayer].position.X;
                        float num3 = player.position.Y - Main.player[myPlayer].position.Y;
                        if ((float)Math.Sqrt((double)(arg * arg + num3 * num3)) < 800f)
                        {
                            Main.player[myPlayer].AddBuff(43, 20, true);
                        }
                    }
                }
            }
            if ((double)player.statLife <= (double)player.statLifeMax2 * 0.5)
            {
                player.AddBuff(62, 5, true);
            }
            if ((double)player.statLife <= (double)player.statLifeMax2 * 0.15)
            {
                player.endurance += 0.05f;
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PaladinsShield);
            recipe.AddIngredient(ItemID.FrozenTurtleShell);
            recipe.AddIngredient(null, "CoreofEleum", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}