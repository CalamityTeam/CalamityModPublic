using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarfleetMK2Gun : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/StarfleetMK2";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starmada");
        }

        public override void SetDefaults()
        {
            projectile.width = 122;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 32f / 255f, 208f / 255f, 255f / 255f);
            Player player = Main.player[projectile.owner];
            projectile.ai[1]--;
            if (projectile.ai[0] >= 0f)
            {
                projectile.ai[0] += 1f;
                switch ((int)projectile.ai[0])
                {
                    case 90:
                    case 180:
                    case 270:
                    case 360:
                    case 450:
                        projectile.localAI[0] += 1f;
                        break;
                    case 540:
                        projectile.localAI[0] += 1f;
                        projectile.ai[0] = -1f;
                        Main.PlaySound(SoundID.Item14, projectile.Center);
                        int dustAmt = 36;
                        for (int d = 0; d < dustAmt; d++)
                        {
                            Vector2 source = Vector2.Normalize(projectile.velocity) * 9f;
                            source = source.RotatedBy((d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt, default) + player.Center;
                            Vector2 dustVel = source - player.Center;
                            int index = Dust.NewDust(source + dustVel, 0, 0, 221, 0f, 0f, 0, default, 4f);
                            Main.dust[index].noGravity = true;
                            Main.dust[index].velocity = dustVel;
                        }
                        break;
                }
            }
            int baseUseTime = 27; //Ranges from 27 to 15 use time
            int modifier = 2;
            bool timeToFire = false;
            if (projectile.ai[1] <= 0f)
            {
                projectile.ai[1] = baseUseTime - modifier * projectile.localAI[0];
                timeToFire = true;
            }
            bool canFire = player.channel && player.HasAmmo(player.ActiveItem(), true) && !player.noItems && !player.CCed;
            if (projectile.soundDelay <= 0 && canFire)
            {
                projectile.soundDelay = baseUseTime - modifier * (int)projectile.localAI[0];
                if (projectile.ai[0] != 1f)
                    Main.PlaySound(SoundID.Item92, projectile.Center);
            }
            if (timeToFire && Main.myPlayer == projectile.owner)
            {
                if (canFire)
                {
                    int type = ModContent.ProjectileType<FallenStarProj>();
                    float shootSpeed = 16f;
                    int damage = player.GetWeaponDamage(player.ActiveItem());
                    float knockBack = player.ActiveItem().knockBack;
                    player.PickAmmo(player.ActiveItem(), ref type, ref shootSpeed, ref canFire, ref damage, ref knockBack, false);
                    for (int i = 0; i < 5; i++)
                    {
                        knockBack = player.GetWeaponKnockback(player.ActiveItem(), knockBack);
                        Vector2 playerPosition = player.RotatedRelativePoint(player.MountedCenter, true);
                        projectile.velocity = Main.screenPosition - playerPosition;
                        projectile.velocity.X += Main.mouseX;
                        projectile.velocity.Y += Main.mouseY;
                        if (player.gravDir == -1f)
                            projectile.velocity.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - playerPosition.Y;
                        projectile.velocity.Normalize();
                        float variation = (1f + projectile.localAI[0]) * 3f;
                        Vector2 position = playerPosition + Utils.RandomVector2(Main.rand, -variation, variation);
                        Vector2 speed = projectile.velocity * shootSpeed * Main.rand.NextFloat(0.6f, 1.2f);
                        type = Utils.SelectRandom(Main.rand, new int[]
                        {
                            ModContent.ProjectileType<PlasmaBlast>(),
                            ModContent.ProjectileType<AstralStar>(),
                            ModContent.ProjectileType<GalacticaComet>(),
                            ModContent.ProjectileType<FallenStarProj>(),
                            ProjectileID.Starfury
                        });
                        int star = Projectile.NewProjectile(position, speed, type, damage, knockBack, projectile.owner);
                        if (star.WithinBounds(Main.maxProjectiles))
                        {
                            Main.projectile[star].penetrate = 1;
                            Main.projectile[star].timeLeft = 300;
                            Main.projectile[star].Calamity().forceRanged = true;
                            Main.projectile[star].netUpdate = true;
                        }
                        projectile.netUpdate = true;
                    }
                }
                else
                {
                    projectile.Kill();
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
            Vector2 displayOffset = new Vector2(27f, -10f * projectile.direction).RotatedBy(projectile.rotation);
            projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + displayOffset;
            if (projectile.spriteDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = projectile.direction;
            projectile.timeLeft = 2;
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
        }

        public override bool CanDamage() => false;
    }
}
