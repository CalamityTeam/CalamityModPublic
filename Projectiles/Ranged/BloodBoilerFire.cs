using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.scale = 2f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 5;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (!playedSound)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                playedSound = true;
            }

            if (Projectile.scale <= 3f)
                Projectile.scale *= 1.01f;

            Lighting.AddLight(Projectile.Center, 1f, 0f, 0f);

            if (Projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (Projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (Projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (Projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                Projectile.ai[0] += 1f;
                int num297 = 5;
                if (Main.rand.NextBool(2))
                {
                    for (int num298 = 0; num298 < 1; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, num297, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 1f);
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
                        dust.velocity += Projectile.velocity;
                        if (!dust.noGravity)
                        {
                            dust.velocity *= 0.5f;
                        }
                    }
                }
            }
            else
            {
                Projectile.ai[0] += 1f;
            }
            Projectile.rotation += 0.3f * (float)Projectile.direction;

            if (Projectile.timeLeft == 160)
                Projectile.ai[1] = 1f;

            if (Projectile.ai[1] == 1f)
            {
                Projectile.tileCollide = false;

                Projectile.extraUpdates = 2;

                Player player = Main.player[Projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 playerCenter = player.Center;
                float xDist = playerCenter.X - Projectile.Center.X;
                float yDist = playerCenter.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                if (dist > 3000f)
                    Projectile.Kill();

                dist = 20f / dist;
                xDist *= dist;
                yDist *= dist;

                // Home back in on the player.
                if (Projectile.velocity.X < xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X + 5f;
                    if (Projectile.velocity.X < 0f && xDist > 0f)
                        Projectile.velocity.X += 5f;
                }
                else if (Projectile.velocity.X > xDist)
                {
                    Projectile.velocity.X = Projectile.velocity.X - 5f;
                    if (Projectile.velocity.X > 0f && xDist < 0f)
                        Projectile.velocity.X -= 5f;
                }
                if (Projectile.velocity.Y < yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 5f;
                    if (Projectile.velocity.Y < 0f && yDist > 0f)
                        Projectile.velocity.Y += 5f;
                }
                else if (Projectile.velocity.Y > yDist)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - 5f;
                    if (Projectile.velocity.Y > 0f && yDist < 0f)
                        Projectile.velocity.Y -= 5f;
                }

                // Delete the projectile if it touches its owner. Has a chance to heal the player again
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Projectile.Hitbox.Intersects(player.Hitbox))
                    {
                        if (Main.rand.NextBool(3) && !Main.player[Projectile.owner].moonLeech)
                        {
                            player.statLife += 1;
                            player.HealEffect(1);
                        }
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BurningBlood>(), 240);

            if (!target.canGhostHeal || Main.player[Projectile.owner].moonLeech)
                return;

            Player player = Main.player[Projectile.owner];
            if (Main.rand.NextBool(2))
            {
                int healAmt = Main.rand.Next(1, 4);
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
        }
    }
}
