using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
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
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.expert = true;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.immune)
            {
                if (Main.rand.NextBool(8))
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
                            int num19 = Projectile.NewProjectile(x, y, num15, num16, ModContent.ProjectileType<AuraRain>(), (int)(18 * player.AverageDamage()), 2f, player.whoAmI, 0f, 0f);
                            Main.projectile[num19].ai[1] = player.position.Y;
                            Main.projectile[num19].tileCollide = false;
							Main.projectile[num19].usesLocalNPCImmunity = true;
							Main.projectile[num19].localNPCHitCooldown = 10;
							Main.projectile[num19].Calamity().forceTypeless = true;
                        }
                    }
                }
            }
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rBrain = true;
        }
    }
}
