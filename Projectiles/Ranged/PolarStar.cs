using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PolarStar : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private int dust1 = 86;
        private int dust2 = 91;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            //Looking pretty at boost two or three
            if (Projectile.ai[1] == 2f || Projectile.ai[1] == 1f)
            {
                Vector2 value7 = new Vector2(5f, 10f);
                Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0.25f);
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] == 48f)
                {
                    Projectile.localAI[0] = 0f;
                }
                else
                {
                    for (int d = 0; d < 2; d++)
                    {
                        int dustType = d == 0 ? dust1 : dust2;
                        Vector2 dustVel = Vector2.UnitX * -12f;
                        dustVel = -Vector2.UnitY.RotatedBy((double)(Projectile.localAI[0] * 0.1308997f + (float)d * MathHelper.Pi), default) * value7;
                        int typeDust = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[typeDust].scale = dustType == dust1 ? 1.5f : 1f;
                        Main.dust[typeDust].noGravity = true;
                        Main.dust[typeDust].position = Projectile.Center + dustVel;
                        Main.dust[typeDust].velocity = Projectile.velocity;
                        int extraDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                        Main.dust[extraDust].noGravity = true;
                        Main.dust[extraDust].velocity *= 0f;
                    }
                }
            }
            if (Projectile.ai[1] == 2f) //Boost three
            {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
            }
            else if (Projectile.ai[1] == 1f) //Boost two
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                float boostLevel = 30f;
                float inc = 1f;
                if (Projectile.ai[1] == 1f)
                {
                    Projectile.localAI[0] += inc;
                    if (Projectile.localAI[0] > boostLevel)
                    {
                        Projectile.localAI[0] = boostLevel;
                    }
                }
                else
                {
                    Projectile.localAI[0] -= inc;
                    if (Projectile.localAI[0] <= 0f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else //No boosts
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                float boostLevel = 40f;
                float inc = 1.5f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.localAI[0] += inc;
                    if (Projectile.localAI[0] > boostLevel)
                    {
                        Projectile.localAI[0] = boostLevel;
                    }
                }
                else
                {
                    Projectile.localAI[0] -= inc;
                    if (Projectile.localAI[0] <= 0f)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor) => Projectile.DrawBeam(40f, 1.5f, lightColor);

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.owner == Main.myPlayer && target.IsAnEnemy(false))
            {
                player.AddBuff(ModContent.BuffType<PolarisBuff>(), 480);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * 0.5f}, Projectile.position);
            if (Projectile.ai[1] == 1f) //Boost two
            {
                int projectiles = Main.rand.Next(2, 5);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < projectiles; k++)
                    {
                        int split = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, (float)Main.rand.Next(-10, 11) * 2f, (float)Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2>(),
                        (int)((double)Projectile.damage * 0.85), (float)(int)((double)Projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (Projectile.ai[1] == 2f) //Boost three
            {
                Projectile.ExpandHitboxBy(150);
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                Projectile.damage /= 2;
                Projectile.Damage();

                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++) // 108 dusts
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.3f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;

                    int i = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool() ? dust1 : dust2, dustVel.X * 0.3f, dustVel.Y * 0.3f, 100, default, 2f);
                    Main.dust[i].noGravity = true;

                    int j = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool() ? dust1 : dust2, dustVel.X * 0.2f, dustVel.Y * 0.2f, 100, default, 2f);
                    Main.dust[j].noGravity = true;

                    int k = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool() ? dust1 : dust2, dustVel.X * 0.1f, dustVel.Y * 0.1f, 100, default, 2f);
                    Main.dust[k].noGravity = true;
                }

                bool random = Main.rand.NextBool();
                float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
                {
                    random = !random;
                    Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center, random ? dust1 : dust2, velocity);
                    d.noGravity = true;
                    d.customData = 0.025f;
                    d.scale = 2f;
                }
            }
        }
    }
}
