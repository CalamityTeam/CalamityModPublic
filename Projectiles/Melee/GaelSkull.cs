using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class GaelSkull : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = GaelsGreatsword.ImmunityFrames;
        }

        public override void AI()
        {
            //Targeting
            if (Projectile.scale <= 1f)
            {
                NPC target = Projectile.Center.ClosestNPCAt(GaelsGreatsword.SearchDistance);
                Projectile.tileCollide = target != null; //Go through walls if we're hunting an NPC

                if (target != null)
                {
                    float distanceFromTarget = Projectile.Distance(target.Center);
                    float homingSpeed = Projectile.velocity.Length() * (distanceFromTarget < 220f ? 1.25f : 1f);
                    if (homingSpeed > 22f)
                        homingSpeed = 22f;

                    Vector2 idealVelocity = Projectile.SafeDirectionTo(target.Center) * homingSpeed;

                    float inertia = Projectile.Distance(target.Center) < 240f ? 4f : 13f;
                    Projectile.velocity = (Projectile.velocity * inertia + idealVelocity) / (inertia + 1f);
                    if (distanceFromTarget < 300f)
                        Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 2f);
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= homingSpeed;
                }
            }

            //Rotation

            if (Projectile.velocity.X < 0f)
            {
                Projectile.spriteDirection = -1;
                Projectile.rotation = (-Projectile.velocity).ToRotation();
            }
            else
            {
                Projectile.spriteDirection = 1;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            //Dust circle

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] % 20 == 0f)
            {
                for (int l = 0; l < 14; l++)
                {
                    Vector2 spawnPositionAdditive = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    spawnPositionAdditive += -Vector2.UnitY.RotatedBy((double)((float)l * MathHelper.TwoPi / 14f), default) * new Vector2(8f, 16f) * Projectile.scale;
                    spawnPositionAdditive = spawnPositionAdditive.RotatedBy((double)(Projectile.rotation), default);
                    int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, 218, 0f, 0f, 0, new Color(188, 126, 154), 1.5f);
                    Main.dust[dustIndex].noGravity = true;
                    Main.dust[dustIndex].position = Projectile.Center + spawnPositionAdditive;
                    Main.dust[dustIndex].velocity = Projectile.velocity * 0.1f;
                    Main.dust[dustIndex].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[dustIndex].position) * 1.25f;
                }
            }

            //Frame logic

            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5 == 0)
            {
                Projectile.frame++;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Fat skull logic
            if (Projectile.ai[1] == 1f)
            {
                Projectile.penetrate = -1;
                Projectile.alpha += 1;
                Projectile.damage = (int)Math.Ceiling(Projectile.damage * 0.992); //Exponentially decays to a factor of 0.12896 of the original damage
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 15;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 240;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            if (Projectile.scale == 1.75f)
                Projectile.damage /= 2;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 218, 0f, 0f, 100, default, 1.5f);
            }
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                int dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 218, 0f, 0f, 0, default, 2.5f);
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].velocity *= 3f;
                dustIndex = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 218, 0f, 0f, 100, default, 1.5f);
                Main.dust[dustIndex].velocity *= 2f;
                Main.dust[dustIndex].noGravity = true;
                Dust.NewDust(Projectile.Center + angle.ToRotationVector2() * 160f, 0, 0, 218, 0f, 0f, 100, default, 1.5f);
            }
        }
    }
}
