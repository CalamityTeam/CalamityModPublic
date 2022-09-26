using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class NebulousCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Nebulous Core");
            Tooltip.SetDefault("10% increased damage\n" +
                               "Summons floating nebula stars to protect you\n" +
                               "You will survive an attack that would have killed you and be healed 100 HP\n" +
                               "This effect has a 90 second cooldown");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float num = Main.rand.Next(90, 111) * 0.01f;
            num *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.35f * num, 0.05f * num, 0.35f * num);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nCore = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            int damage = (int)player.GetBestClassDamage().ApplyTo(250);
            float knockBack = 3f;
            if (Main.rand.NextBool(15))
            {
                int num = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<NebulaStar>())
                    {
                        num++;
                    }
                }
                var source = player.GetSource_Accessory(Item);
                if (Main.rand.Next(15) >= num && num < 10)
                {
                    int num2 = 50;
                    int num3 = 24;
                    int num4 = 90;
                    for (int j = 0; j < num2; j++)
                    {
                        int num5 = Main.rand.Next(200 - j * 2, 400 + j * 2);
                        Vector2 center = player.Center;
                        center.X += (float)Main.rand.Next(-num5, num5 + 1);
                        center.Y += (float)Main.rand.Next(-num5, num5 + 1);
                        if (!Collision.SolidCollision(center, num3, num3) && !Collision.WetCollision(center, num3, num3))
                        {
                            center.X += (float)(num3 / 2);
                            center.Y += (float)(num3 / 2);
                            if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                            {
                                int num6 = (int)center.X / 16;
                                int num7 = (int)center.Y / 16;
                                bool flag = false;
                                if (Main.rand.NextBool(3) && Main.tile[num6, num7] != null && Main.tile[num6, num7].WallType > 0)
                                {
                                    flag = true;
                                }
                                else
                                {
                                    center.X -= (float)(num4 / 2);
                                    center.Y -= (float)(num4 / 2);
                                    if (Collision.SolidCollision(center, num4, num4))
                                    {
                                        center.X += (float)(num4 / 2);
                                        center.Y += (float)(num4 / 2);
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    for (int k = 0; k < Main.maxProjectiles; k++)
                                    {
                                        if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<NebulaStar>() && (center - Main.projectile[k].Center).Length() < 48f)
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                    if (flag && Main.myPlayer == player.whoAmI)
                                    {
                                        Projectile.NewProjectile(source, center.X, center.Y, 0f, 0f, ModContent.ProjectileType<NebulaStar>(), damage, knockBack, player.whoAmI);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
