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
    public class PlagueHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Hive");
            Tooltip.SetDefault("Blinds and weakens the player if above 90% life\n" +
                "Summons a damaging plague aura around the player to destroy nearby enemies\n" +
                "Releases bees when damaged\n" +
                "Friendly bees inflict the plague\n" +
                "All of your attacks inflict the plague\n" +
                "Makes you immune to the plague\n" +
                "Projectiles spawn plague seekers on enemy hits\n" +
                "The power of your bees and wasps will rival the Moon Lord himself");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = 500000;
            item.expert = true;
            item.accessory = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ToxicHeart");
            recipe.AddIngredient(null, "AlchemicalFlask");
            recipe.AddIngredient(ItemID.HiveBackpack);
            recipe.AddIngredient(ItemID.HoneyComb);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            player.buffImmune[mod.BuffType("Plague")] = true;
            player.bee = true;
            modPlayer.uberBees = true;
            modPlayer.alchFlask = true;
            if (player.statLife >= (player.statLifeMax * 0.9f))
            {
                player.blind = true;
                player.statDefense -= 5;
                player.moveSpeed -= 0.05f;
            }
            int plagueCounter = 0;
            Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.1f, 2f, 0.2f);
            int num = mod.BuffType("Plague");
            float num2 = 300f;
            bool flag = plagueCounter % 60 == 0;
            int num3 = 60;
            int random = Main.rand.Next(10);
            if (player.whoAmI == Main.myPlayer)
            {
                if (random == 0)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (nPC.FindBuffIndex(num) == -1)
                            {
                                nPC.AddBuff(num, 120, false);
                            }
                            if (flag)
                            {
                                nPC.StrikeNPC(num3, 0f, 0, false, false, false);
                                if (Main.netMode != 0)
                                {
                                    NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            plagueCounter++;
            if (plagueCounter >= 180)
            {
                plagueCounter = 0;
            }
        }
    }
}