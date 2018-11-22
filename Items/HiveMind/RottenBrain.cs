using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.HiveMind
{
    public class RottenBrain : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rotten Brain");
            Tooltip.SetDefault("10% increased damage when below 75% life\n5% decreased movement speed when below 50% life\nShade rains down when you are hit");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.value = 100000;
            item.expert = true;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
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
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, mod.ProjectileType("AuraRain"), 18, 2f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].ai[1] = player.position.Y;
                            Main.projectile[num19].tileCollide = false;
                        }
                    }
                }
            }
            if (player.statLife <= (player.statLifeMax2 * 0.75f))
            {
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.thrownDamage += 0.1f;
                player.minionDamage += 0.1f;
            }
            if (player.statLife <= (player.statLifeMax2 * 0.5f))
            {
                player.moveSpeed -= 0.05f;
            }
        }
    }
}