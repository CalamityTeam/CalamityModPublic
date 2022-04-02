using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class BloodBoilerFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool playedSound = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.scale = 2f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 5;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!playedSound)
            {
                Main.PlaySound(SoundID.Item34, (int)projectile.position.X, (int)projectile.position.Y);
                playedSound = true;
            }

            if (projectile.scale <= 3f)
                projectile.scale *= 1.01f;

            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);

            if (projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = 5;
                if (Main.rand.NextBool(2))
                {
                    for (int num298 = 0; num298 < 1; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                        Dust dust = Main.dust[num299];
                        if (Main.rand.NextBool(3))
                        {
                            dust.noGravity = true;
                            dust.scale *= 3f;
                            dust.velocity.X *= 2f;
                            dust.velocity.Y *= 2f;
                        }
                        else
                        {
                            dust.scale *= 1.5f;
                        }
                        dust.velocity.X *= 1.2f;
                        dust.velocity.Y *= 1.2f;
                        dust.scale *= num296;
                        dust.velocity += projectile.velocity;
                        if (!dust.noGravity)
                        {
                            dust.velocity *= 0.5f;
                        }
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;

            if (projectile.timeLeft == 160)
                projectile.ai[1] = 1f;

            if (projectile.ai[1] == 1f)
            {
                projectile.tileCollide = false;

                projectile.extraUpdates = 2;

                Player player = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = player.Center;
                float xDist = playerCenter.X - projectile.Center.X;
                float yDist = playerCenter.Y - projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    projectile.Kill();

                dist = 20f / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (projectile.velocity.X < xDist)
                {
                    projectile.velocity.X = projectile.velocity.X + 5f;
                    if (projectile.velocity.X < 0f && xDist > 0f)
                        projectile.velocity.X += 5f;
                }
                else if (projectile.velocity.X > xDist)
                {
                    projectile.velocity.X = projectile.velocity.X - 5f;
                    if (projectile.velocity.X > 0f && xDist < 0f)
                        projectile.velocity.X -= 5f;
                }
                if (projectile.velocity.Y < yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y + 5f;
                    if (projectile.velocity.Y < 0f && yDist > 0f)
                        projectile.velocity.Y += 5f;
                }
                else if (projectile.velocity.Y > yDist)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 5f;
                    if (projectile.velocity.Y > 0f && yDist < 0f)
                        projectile.velocity.Y -= 5f;
                }

                // Delete the projectile if it touches its owner. Has a chance to heal the player again
                if (Main.myPlayer == projectile.owner)
                {
                    if (projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        if (Main.rand.NextBool(3) && !Main.player[projectile.owner].moonLeech)
                        {
                            player.statLife += 1;
                            player.HealEffect(1);
                        }
                        projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 240);

            if (!target.canGhostHeal || Main.player[projectile.owner].moonLeech)
                return;

            Player player = Main.player[projectile.owner];
            if (Main.rand.NextBool(2))
            {
                int healAmt = Main.rand.Next(1, 4);
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
        }
    }
}
