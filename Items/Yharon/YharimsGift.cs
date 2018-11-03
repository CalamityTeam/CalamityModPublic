using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Yharon
{
    public class YharimsGift : ModItem
    {
        public int dragonTimer = 60;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Gift");
            Tooltip.SetDefault("The power of a god pulses from within this artifact\n" +
                               "Flaming meteors rain down while invincibility is active\n" +
                               "Exploding dragon dust is left behind as you move\n" +
                               "Defense increased by 30 and damage increased by 15%");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = 50000000;
            item.accessory = true;
            item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.meleeDamage += 0.15f;
            player.thrownDamage += 0.15f;
            player.rangedDamage += 0.15f;
            player.magicDamage += 0.15f;
            player.minionDamage += 0.15f;
            player.statDefense += 30;
            if (((double)player.velocity.X > 0 || (double)player.velocity.Y > 0 || (double)player.velocity.X < -0.1 || (double)player.velocity.Y < -0.1))
            {
                dragonTimer--;
                if (dragonTimer <= 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        int projectile1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("DragonDust"), 350, 5f, player.whoAmI, 0f, 0f);
                        Main.projectile[projectile1].timeLeft = 60;
                    }
                    dragonTimer = 60;
                }
            }
            else
            {
                dragonTimer = 60;
            }
            if (player.immune)
            {
                if (Main.rand.Next(8) == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (int l = 0; l < 1; l++)
                        {
                            float x = player.position.X + (float)Main.rand.Next(-400, 400);
                            float y = player.position.Y - (float)Main.rand.Next(500, 800);
                            Vector2 vector = new Vector2(x, y);
                            float num15 = player.position.X + (float)(player.width / 2) - vector.X;
                            float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
                            num15 += (float)Main.rand.Next(-100, 101);
                            int num17 = 22;
                            float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                            num18 = (float)num17 / num18;
                            num15 *= num18;
                            num16 *= num18;
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, mod.ProjectileType("SkyFlareFriendly"), 750, 9f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].ai[1] = player.position.Y;
                            Main.projectile[num19].hostile = false;
                            Main.projectile[num19].friendly = true;
                        }
                    }
                }
            }
        }
    }
}