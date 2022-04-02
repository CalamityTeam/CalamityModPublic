using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Ranged
{
    public class PolarStar : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private int dust1 = 86;
        private int dust2 = 91;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polar Star");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
        }

        public override void AI()
        {
            //Looking pretty at boost two or three
            if (projectile.ai[1] == 2f || projectile.ai[1] == 1f)
            {
                Vector2 value7 = new Vector2(5f, 10f);
                Lighting.AddLight(projectile.Center, 0.25f, 0f, 0.25f);
                projectile.localAI[0] += 1f;
                if (projectile.localAI[0] == 48f)
                {
                    projectile.localAI[0] = 0f;
                }
                else
                {
                    for (int d = 0; d < 2; d++)
                    {
                        int dustType = d == 0 ? dust1 : dust2;
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy((double)(projectile.localAI[0] * 0.1308997f + (float)d * MathHelper.Pi), default) * value7;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = dustType == dust1 ? 1.5f : 1f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                        int num458 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                        Main.dust[num458].noGravity = true;
                        Main.dust[num458].velocity *= 0f;
                    }
                }
            }
            if (projectile.ai[1] == 2f) //Boost three
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 200f, 12f, 20f);
            }
            else if (projectile.ai[1] == 1f) //Boost two
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                float num55 = 30f;
                float num56 = 1f;
                if (projectile.ai[1] == 1f)
                {
                    projectile.localAI[0] += num56;
                    if (projectile.localAI[0] > num55)
                    {
                        projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    projectile.localAI[0] -= num56;
                    if (projectile.localAI[0] <= 0f)
                    {
                        projectile.Kill();
                    }
                }
            }
            else //No boosts
            {
                if (projectile.alpha > 0)
                {
                    projectile.alpha -= 25;
                }
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                float num55 = 40f;
                float num56 = 1.5f;
                if (projectile.ai[1] == 0f)
                {
                    projectile.localAI[0] += num56;
                    if (projectile.localAI[0] > num55)
                    {
                        projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    projectile.localAI[0] -= num56;
                    if (projectile.localAI[0] <= 0f)
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) => projectile.DrawBeam(40f, 1.5f, lightColor);

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[projectile.owner];
            if ((target.damage > 5 || target.boss) && projectile.owner == Main.myPlayer && !target.SpawnedFromStatue)
            {
                player.AddBuff(ModContent.BuffType<PolarisBuff>(), 180);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 62, 0.5f);
            if (projectile.ai[1] == 1f) //Boost two
            {
                int projectiles = Main.rand.Next(2, 5);
                if (projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < projectiles; k++)
                    {
                        int split = Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-10, 11) * 2f, (float)Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2>(),
                        (int)((double)projectile.damage * 0.85), (float)(int)((double)projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (projectile.ai[1] == 2f) //Boost three
            {
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, 150);
                projectile.maxPenetrate = -1;
                projectile.penetrate = -1;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = 10;
                projectile.damage /= 2;
                projectile.Damage();

                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++) // 108 dusts
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.3f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;

                    int i = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.3f, dustVel.Y * 0.3f, 100, default, 2f);
                    Main.dust[i].noGravity = true;

                    int j = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.2f, dustVel.Y * 0.2f, 100, default, 2f);
                    Main.dust[j].noGravity = true;

                    int k = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.1f, dustVel.Y * 0.1f, 100, default, 2f);
                    Main.dust[k].noGravity = true;
                }

                bool random = Main.rand.NextBool();
                float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
                {
                    random = !random;
                    Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust d = Dust.NewDustPerfect(projectile.Center, random ? dust1 : dust2, velocity);
                    d.noGravity = true;
                    d.customData = 0.025f;
                    d.scale = 2f;
                }
            }
        }
    }
}
