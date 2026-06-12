using System;
using System.IO;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyBlast : ModProjectile, ILocalizedModType
    {
        public bool started = false;
        public int FireDamage = 0;

        private const int TimeLeft = 120;
        private const int AccelerationTime = 60;
        private const float Acceleration = 1.05f;

        public new string LocalizationCategory => "Projectiles.Boss";
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastShoot");
        public static readonly SoundStyle ImpactSound = new("CalamityMod/Sounds/Custom/Providence/ProvidenceHolyBlastImpact");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = TimeLeft;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.scale = 0;
        }

        public override void OnSpawn(IEntitySource source)
        {
            FireDamage = Providence.FireDamage.CalculateProvidenceDamage();

            if (source is EntitySource_Parent { Entity: NPC parent })
            {
                if (parent.type == ModContent.NPCType<ProfanedGuardianCommander>())
                    FireDamage = ProfanedGuardianCommander.FireDamage;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.scale < 1)
                Projectile.scale += 0.05f;
            ProvUtils.ApplyGFBDamage(Projectile, 180, 20);

            Lighting.AddLight(Projectile.Center, 0.9f, 0.7f, 0f);

            if (Projectile.timeLeft > TimeLeft - AccelerationTime && Projectile.ai[2] == 0f)
                Projectile.velocity *= Acceleration;

            if (Projectile.Hitbox.Intersects(new Rectangle((int)Projectile.ai[0], (int)Projectile.ai[1], Player.defaultWidth, Player.defaultHeight)))
                Projectile.tileCollide = true;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            if (!started)
            {
                for (int i = 0; i < 13; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new PointParticle(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-30f, 30f))) * Main.rand.NextFloat(2.4f, 4.2f), false, 15, Main.rand.NextFloat(3f, 6f), ProvUtils.GetProjectileColor(255, false)));
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Projectile.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-50f, 50f))) * Main.rand.NextFloat(2.4f, 4.2f), false, 15, Main.rand.NextFloat(2f, 5f), ProvUtils.GetProjectileColor(255, false)));
                }
                started = true;
            }

            if (Projectile.localAI[0] == 0f)
            {
                int dustType = ProvUtils.GetDustID();
                for (int i = 0; i < 10; i++)
                {
                    int holyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                    Main.dust[holyDust].velocity *= 3f;
                    Main.dust[holyDust].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[holyDust].scale = 0.5f;
                        Main.dust[holyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.localAI[0] = 1f;
                SoundEngine.PlaySound(ShootSound, Projectile.Center);
            }

            Projectile.direction = Projectile.spriteDirection = (Math.Abs(Projectile.velocity.X) > 0.2).ToDirectionInt();

            GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(50), 0).RotatedByRandom(MathHelper.TwoPi), Projectile.velocity.RotatedBy(Math.PI) * 0.6f, false, 20, Main.rand.NextFloat(1f, 2f), ProvUtils.GetProjectileColor(255)));
            GeneralParticleHandler.SpawnParticle(new MediumMistParticle(Projectile.Center + new Vector2(40, 0).RotatedBy(Vector2.Zero.AngleTo(-Projectile.velocity)) + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi), Projectile.velocity.RotatedBy(Math.PI) * 0.6f, Color.LightSlateGray, Color.DarkSlateGray, Main.rand.NextFloat(1f, 3f), 150f));

            if (Projectile.velocity.X < 0f)
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
            else
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
        }

        public override Color? GetAlpha(Color lightColor) => ProvUtils.GetProjectileColor(lightColor);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ProvUtils.StandardAI() ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            SpriteEffects sp = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(lightColor, true), 4f, texture, effects: sp);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color hiColor = ProvUtils.GetProjectileColor(255, false);
            Color loColor = ProvUtils.GetProjectileColor(0, true);

            for (int i = 0; i < 30; i++)
            {
                Particle p = new FlameParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(150), 0).RotatedByRandom(MathHelper.TwoPi), 40, Main.rand.NextFloat(0.5f, 0.75f), Main.rand.NextFloat(1f, 2.5f), hiColor, loColor);
                p.Velocity = new Vector2(Main.rand.NextFloat(3f, 19f), 0).RotatedByRandom(MathHelper.TwoPi);
                GeneralParticleHandler.SpawnParticle(p);
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(12f, 40f), 0).RotatedByRandom(MathHelper.TwoPi), loColor, 60, Main.rand.NextFloat(0.75f, 1.75f), 1f, Main.rand.NextFloat(-0.05f, 0.05f), true));
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/BloomCircle", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 1f, 0.1f, 15));

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.25f, 35));
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.05f, 0.175f, 25));

            if (Projectile.owner == Main.myPlayer)
            {
                int totalProjectiles = !ProvUtils.StandardAI() ? 8 : 6;
                if (Main.getGoodWorld)
                    totalProjectiles *= 2;

                float radians = MathHelper.TwoPi / totalProjectiles;
                int type = ModContent.ProjectileType<HolyBlastFrags>();
                float velocity = 5f;
                Vector2 spinningPoint = new Vector2(0f, -velocity);
                Vector2 additionalVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 2.5f;
                for (int k = 0; k < totalProjectiles; k++)
                {
                    Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity2 + additionalVelocity, type, FireDamage, 0f, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(ImpactSound, Projectile.Center);

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);

            int dustType = ProvUtils.GetDustID();
            for (int j = 0; j < 4; j++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 2f);
                Main.dust[dust].noGravity = true;
            }
            for (int k = 0; k < 40; k++)
            {
                int profaned = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 4f);
                Main.dust[profaned].noGravity = true;
                Main.dust[profaned].velocity *= 3f;
                profaned = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 50, default, 2f);
                Main.dust[profaned].velocity *= 2f;
                Main.dust[profaned].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if (info.Damage <= 0 || target.creativeGodMode)
                return;

            ProvUtils.ApplyDebuffs(target, 180);
        }
    }
}
