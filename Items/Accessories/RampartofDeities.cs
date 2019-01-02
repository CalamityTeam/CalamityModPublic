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
    public class RampartofDeities : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rampart of Deities");
            Tooltip.SetDefault("Taking damage makes you move very fast for a short time\n" +
                "Increases armor penetration by 50 and immune time after being struck\n" +
                "Provides light underwater and causes stars to fall when damaged\n" +
                "Increases pickup range for mana stars and you restore mana when damaged\n" +
                "Absorbs 25% of damage done to players on your team\n" +
                "Only active above 25% life\n" +
                "Grants immunity to knockback\n" +
                "Puts a shell around the owner when below 50% life that reduces damage\n" +
                "The shell becomes more powerful when below 15% life and reduces damage even further");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 44;
            item.value = Item.buyPrice(0, 90, 0, 0);
            item.defense = 12;
            item.accessory = true;
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

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.dAmulet = true;
            player.panic = true;
            player.armorPenetration += 50;
            player.manaMagnet = true;
            player.magicCuffs = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 1.35f, 0.3f, 0.9f);
            }
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
            recipe.AddIngredient(null, "FrigidBulwark");
            recipe.AddIngredient(null, "DeificAmulet");
            recipe.AddIngredient(null, "GalacticaSingularity", 5);
            recipe.AddIngredient(null, "DivineGeode", 10);
            recipe.AddIngredient(null, "CosmiliteBar", 20);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}