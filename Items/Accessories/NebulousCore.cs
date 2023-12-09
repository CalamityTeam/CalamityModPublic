using CalamityMod.CalPlayer;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class NebulousCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
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
            float projLighting = Main.rand.Next(90, 111) * 0.01f;
            projLighting *= Main.essScale;
            Lighting.AddLight((int)((Item.position.X + (float)(Item.width / 2)) / 16f), (int)((Item.position.Y + (float)(Item.height / 2)) / 16f), 0.35f * projLighting, 0.05f * projLighting, 0.35f * projLighting);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nCore = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
            int damage = (int)player.GetBestClassDamage().ApplyTo(250);
            damage = player.ApplyArmorAccDamageBonusesTo(damage);
            float knockBack = 3f;
            if (Main.rand.NextBool(15))
            {
                int numProj = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<NebulaStar>())
                    {
                        numProj++;
                    }
                }
                var source = player.GetSource_Accessory(Item);
                if (Main.rand.Next(15) >= numProj && numProj < 10)
                {
                    int spawnRadius = 24;
                    int backupSpawnRadius = 90;
                    for (int j = 0; j < 50; j++)
                    {
                        int randomProjOffset = Main.rand.Next(200 - j * 2, 400 + j * 2);
                        Vector2 center = player.Center;
                        center.X += (float)Main.rand.Next(-randomProjOffset, randomProjOffset + 1);
                        center.Y += (float)Main.rand.Next(-randomProjOffset, randomProjOffset + 1);
                        if (!Collision.SolidCollision(center, spawnRadius, spawnRadius) && !Collision.WetCollision(center, spawnRadius, spawnRadius))
                        {
                            center.X += (float)(spawnRadius / 2);
                            center.Y += (float)(spawnRadius / 2);
                            if (Collision.CanHit(new Vector2(player.Center.X, player.position.Y), 1, 1, center, 1, 1) || Collision.CanHit(new Vector2(player.Center.X, player.position.Y - 50f), 1, 1, center, 1, 1))
                            {
                                int projTileX = (int)center.X / 16;
                                int projTileY = (int)center.Y / 16;
                                bool canSpawnProj = false;
                                if (Main.rand.NextBool(3) && Main.tile[projTileX, projTileY] != null && Main.tile[projTileX, projTileY].WallType > 0)
                                {
                                    canSpawnProj = true;
                                }
                                else
                                {
                                    center.X -= (float)(backupSpawnRadius / 2);
                                    center.Y -= (float)(backupSpawnRadius / 2);
                                    if (Collision.SolidCollision(center, backupSpawnRadius, backupSpawnRadius))
                                    {
                                        center.X += (float)(backupSpawnRadius / 2);
                                        center.Y += (float)(backupSpawnRadius / 2);
                                        canSpawnProj = true;
                                    }
                                }
                                if (canSpawnProj)
                                {
                                    for (int k = 0; k < Main.maxProjectiles; k++)
                                    {
                                        if (Main.projectile[k].active && Main.projectile[k].owner == player.whoAmI && Main.projectile[k].type == ModContent.ProjectileType<NebulaStar>() && (center - Main.projectile[k].Center).Length() < 48f)
                                        {
                                            canSpawnProj = false;
                                            break;
                                        }
                                    }
                                    if (canSpawnProj && Main.myPlayer == player.whoAmI)
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
