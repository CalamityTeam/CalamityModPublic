using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Events;
using CalamityMod.World;

namespace CalamityMod.Projectiles.Boss
{
    public class AstralFlame : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 100;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;

            if (Main.getGoodWorld)
                Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (float)Math.Atan2((double)-(double)Projectile.velocity.Y, (double)-(double)Projectile.velocity.X);
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);
            }

            Lighting.AddLight(Projectile.Center, 0.3f, 0.5f, 0.1f);

            int targetPlayer = Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 playerDist = Main.player[targetPlayer].Center - Projectile.Center;
            if (playerDist.Length() < 60f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 15f)
            {
                int astralDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 0.8f);
                Main.dust[astralDust].noGravity = true;
                Main.dust[astralDust].velocity *= 0f;
            }

            if (Projectile.ai[0] >= 120f)
            {
                if (Projectile.ai[1] < 180f)
                {
                    float scaleFactor2 = Projectile.velocity.Length();
                    playerDist.Normalize();
                    playerDist *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 30f + playerDist) / 31f;
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= scaleFactor2;
                }
                else if (Projectile.velocity.Length() < (death ? 15f : 12f))
                    Projectile.velocity *= death ? 1.015f : 1.01f;

                Projectile.ai[1] += 1f;
            }
            else
            {
                // Move away from other flames
                if (Projectile.ai[0] >= 60f)
                {
                    float pushForce = bossRush ? 0.07f : death ? 0.06f : revenge ? 0.055f : expertMode ? 0.05f : 0.04f;
                    float pushDistance = bossRush ? 180f : death ? 150f : revenge ? 135f : expertMode ? 120f : 90f;
                    for (int k = 0; k < Main.maxProjectiles; k++)
                    {
                        Projectile otherProj = Main.projectile[k];

                        // Short circuits to make the loop as fast as possible
                        if (!otherProj.active || k == Projectile.whoAmI)
                            continue;

                        // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away
                        bool sameProjType = otherProj.type == Projectile.type;
                        float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                        if (sameProjType && taxicabDist < pushDistance)
                        {
                            if (Projectile.position.X < otherProj.position.X)
                                Projectile.velocity.X -= pushForce;
                            else
                                Projectile.velocity.X += pushForce;

                            if (Projectile.position.Y < otherProj.position.Y)
                                Projectile.velocity.Y -= pushForce;
                            else
                                Projectile.velocity.Y += pushForce;
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 75);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 96;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 50, default, 1f);
            }
            for (int j = 0; j < 20; j++)
            {
                int deathAstralDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 0, default, 1.5f);
                Main.dust[deathAstralDust].noGravity = true;
                Main.dust[deathAstralDust].velocity *= 3f;
                deathAstralDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 173, 0f, 0f, 50, default, 1f);
                Main.dust[deathAstralDust].velocity *= 2f;
                Main.dust[deathAstralDust].noGravity = true;
            }
            Projectile.Damage();
        }
    }
}
