using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ElementalExcaliburBeam : ModProjectile
    {
        private int alpha = 50;
        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beam");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 3;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
            projectile.timeLeft = 1200;
            projectile.extraUpdates = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            if (!playedSound)
            {
                Main.PlaySound(SoundID.Item60, projectile.Center);
                playedSound = true;
            }
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 15;
                if (projectile.alpha >= 250)
                {
                    projectile.alpha = 255;
                    projectile.localAI[0] = 1f;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 15;
                if (projectile.alpha <= 0)
                {
                    projectile.alpha = 0;
                    projectile.localAI[0] = 0f;
                }
            }

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red, normal beam
                    break;

                case 1: // Orange, curve back to player
                    color = new Color(255, 128, 0, alpha);

                    int p = (int)Player.FindClosest(projectile.Center, 1, 1);
                    projectile.ai[1] += 1f;
                    if (projectile.ai[1] < 220f && projectile.ai[1] > 60f)
                    {
                        float homeSpeed = projectile.velocity.Length();
                        Vector2 vecToPlayer = Main.player[p].Center - projectile.Center;
                        vecToPlayer.Normalize();
                        vecToPlayer *= homeSpeed;
                        projectile.velocity = (projectile.velocity * 24f + vecToPlayer) / 25f;
                        projectile.velocity.Normalize();
                        projectile.velocity *= homeSpeed;
                    }

                    if (projectile.velocity.Length() < 24f)
                    {
                        projectile.velocity *= 1.02f;
                    }

                    break;

                case 2: // Yellow, split after a certain time
                    color = new Color(255, 255, 0, alpha);

                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] >= 180f)
                    {
                        projectile.localAI[1] = 0f;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(10);
                        if (projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj + 1; i++)
                            {
                                Vector2 perturbedSpeed = projectile.velocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                                Projectile.NewProjectile(projectile.Center, perturbedSpeed * 0.5f, ModContent.ProjectileType<ElementalExcaliburBeam>(), (int)(projectile.damage * 0.5), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
                            }
                        }
                        projectile.Kill();
                    }

                    break;

                case 3: // Lime, home in on player within certain distance
                    color = new Color(128, 255, 0, alpha);

                    float inertia = 75f;
                    float homingSpeed = 7.5f;
                    float minDist = 80f;
                    if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
                    {
                        if (projectile.Distance(Main.player[projectile.owner].Center) > minDist)
                        {
                            Vector2 moveDirection = projectile.SafeDirectionTo(Main.player[projectile.owner].Center, Vector2.UnitY);
                            projectile.velocity = (projectile.velocity * (inertia - 1f) + moveDirection * homingSpeed) / inertia;
                        }
                    }
                    else
                    {
                        if (projectile.timeLeft > 30)
                        {
                            projectile.timeLeft = 30;
                        }
                    }

                    break;

                case 4: // Green, home in on enemies
                    color = new Color(0, 255, 0, alpha);

                    CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 6f, 20f);

                    break;

                case 5: // Turquoise, speed up and don't collide with tiles
                    color = new Color(0, 255, 128, alpha);

                    projectile.tileCollide = false;
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 28f)
                    {
                        projectile.velocity *= 1.035f;
                    }

                    break;

                case 6: // Cyan, upon death create several turquoise swords
                    color = new Color(0, 255, 255, alpha);
                    break;

                case 7: // Light Blue, slow down on hit
                    color = new Color(0, 128, 255, alpha);
                    break;

                case 8: // Blue, bounce off of tiles and enemies
                    color = new Color(0, 0, 255, alpha);
                    break;

                case 9: // Purple, speed up and slow down over and over
                    color = new Color(128, 0, 255, alpha);

                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] <= 40f)
                    {
                        projectile.velocity *= 0.95f;
                    }
                    else if (projectile.localAI[1] > 40f && projectile.localAI[1] <= 79f)
                    {
                        projectile.velocity *= 1.05f;
                    }
                    else if (projectile.localAI[1] == 80f)
                    {
                        projectile.localAI[1] = 0f;
                    }

                    break;

                case 10: // Fuschia, start slow then get faster
                    color = new Color(255, 0, 255, alpha);

                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.velocity *= 0.1f;
                        projectile.localAI[1] += 1f;
                    }
                    projectile.velocity *= 1.01f;

                    break;

                case 11: // Hot Pink, split into fuschia while travelling
                    color = new Color(255, 0, 128, alpha);

                    if (projectile.localAI[1] == 0f)
                    {
                        projectile.velocity *= 0.33f;
                    }
                    projectile.localAI[1] += 1f;
                    if (projectile.localAI[1] >= 181f)
                    {
                        projectile.localAI[1] = 1f;
                        if (Main.myPlayer == projectile.owner)
                            Projectile.NewProjectile(projectile.Center + projectile.velocity, projectile.velocity, ModContent.ProjectileType<ElementalExcaliburBeam>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, 10f, 0f);
                    }

                    break;

                default:
                    break;
            }

            if (Main.rand.NextBool(2))
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs(2f);

            if (projectile.ai[0] == 7f)
            {
                projectile.velocity *= 0.25f;
            }
            else if (projectile.ai[0] == 8f)
            {
                projectile.velocity *= -1f;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs(2f);

            if (projectile.ai[0] == 7f)
            {
                projectile.velocity *= 0.25f;
            }
            else if (projectile.ai[0] == 8f)
            {
                projectile.velocity *= -1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.ai[0] == 8f)
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;
                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);
                    break;
                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }
            return color;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.timeLeft > 1195)
                return false;

            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.Center);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 64);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            Color color = new Color(255, 0, 0, alpha);
            switch ((int)projectile.ai[0])
            {
                case 0: // Red
                    break;
                case 1: // Orange
                    color = new Color(255, 128, 0, alpha);
                    break;
                case 2: // Yellow
                    color = new Color(255, 255, 0, alpha);
                    break;
                case 3: // Lime
                    color = new Color(128, 255, 0, alpha);
                    break;
                case 4: // Green
                    color = new Color(0, 255, 0, alpha);
                    break;
                case 5: // Turquoise
                    color = new Color(0, 255, 128, alpha);
                    break;

                case 6: // Cyan
                    color = new Color(0, 255, 255, alpha);

                    for (int x = 0; x < 3; x++)
                    {
                        bool fromRight = x == 1;
                        if (x == 2)
                            fromRight = Main.rand.NextBool(2);
                        if (projectile.owner == Main.myPlayer)
                        {
                            CalamityUtils.ProjectileBarrage(projectile.Center, projectile.Center, fromRight, 500f, 500f, 0f, 500f, 5f, ModContent.ProjectileType<ElementalExcaliburBeam>(), (int)(projectile.damage * 0.2), projectile.knockBack * 0.2f, projectile.owner, false, 0f).ai[0] = 5f;
                        }
                    }

                    break;

                case 7: // Light Blue
                    color = new Color(0, 128, 255, alpha);
                    break;
                case 8: // Blue
                    color = new Color(0, 0, 255, alpha);
                    break;
                case 9: // Purple
                    color = new Color(128, 0, 255, alpha);
                    break;
                case 10: // Fuschia
                    color = new Color(255, 0, 255, alpha);
                    break;
                case 11: // Hot Pink
                    color = new Color(255, 0, 128, alpha);
                    break;
                default:
                    break;
            }

            for (int d = 0; d < 3; d++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].noGravity = true;
            }
            for (int d = 0; d < 30; d++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 2.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 3f;
                dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 267, 0f, 0f, alpha, color, 1.5f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
