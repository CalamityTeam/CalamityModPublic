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
    public class MoltenBlast : ModProjectile, ILocalizedModType
    {
        public int BlobDamage = 0;

        private const int TimeLeft = 90;
        private const int AccelerationTime = 60;
        private const float Acceleration = 1.05f;

        public new string LocalizationCategory => "Projectiles.Boss";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = TimeLeft;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void OnSpawn(IEntitySource source)
        {
            BlobDamage = Providence.BlobDamage.CalculateProvidenceDamage();

            if (source is EntitySource_Parent { Entity: NPC parent })
            {
                if (parent.type == ModContent.NPCType<ProfanedGuardianDefender>())
                    BlobDamage = ProfanedGuardianDefender.BlobDamage;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            ProvUtils.ApplyGFBDamage(Projectile, 120, 20);

            Lighting.AddLight(Projectile.Center, 0.45f, 0.35f, 0f);

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

            int dustType = ProvUtils.GetDustID();
            if (Projectile.localAI[1] == 0f)
            {
                for (int d = 0; d < 10; d++)
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

                Projectile.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item73, Projectile.Center);
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 30f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotate = Vector2.UnitX * -Projectile.width / 2f;
                    dustRotate += -Vector2.UnitY.RotatedBy(l * MathHelper.Pi / 6f, default) * new Vector2(8f, 16f);
                    dustRotate = dustRotate.RotatedBy(Projectile.rotation - MathHelper.PiOver2, default);
                    int profaned = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                    Main.dust[profaned].scale = 1.1f;
                    Main.dust[profaned].noGravity = true;
                    Main.dust[profaned].position = Projectile.Center + dustRotate;
                    Main.dust[profaned].velocity = Projectile.velocity * 0.1f;
                    Main.dust[profaned].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[profaned].position) * 1.25f;
                }
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            float vel = Math.Clamp(((Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) / 2), 0f, 1f);

            GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(15), 0).RotatedByRandom(MathHelper.TwoPi), Projectile.velocity.RotatedBy(Math.PI) * 0.5f, false, 10, Main.rand.NextFloat(0.8f, 1.2f), ProvUtils.GetProjectileColor(255)));

            if (Main.rand.NextBool())
                GeneralParticleHandler.SpawnParticle(new MediumMistParticle(Projectile.Center, Vector2.Zero, Color.LightSlateGray, Color.DarkSlateGray, Main.rand.NextFloat(vel), 150, MathHelper.ToRadians(Main.rand.NextFloat(-1f, 1f))));
        }

        public override Color? GetAlpha(Color lightColor) => ProvUtils.GetProjectileColor(0);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ProvUtils.StandardAI() ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/MoltenBlastNight").Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color hiColor = ProvUtils.GetProjectileColor(255, false);
            Color loColor = ProvUtils.GetProjectileColor(0, true);

            for (int i = 0; i < 25; i++)
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(10), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Main.rand.NextFloat(0.8f, 1.2f), hiColor));

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.5f, 0.1f, 4));

            for (float i = 0; i < 1; i += 0.25f)
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f * i, 0.125f * i, 8));

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.02f, 0.045f, 5));

            int blobAmt = !ProvUtils.StandardAI() ? 9 : 6;
            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 additionalBlobVelocity = new Vector2(Projectile.velocity.X, Projectile.velocity.Y);
                for (int b = 0; b < blobAmt; b++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f) + additionalBlobVelocity;
                    velocity.Y = -Math.Abs(velocity.Y);
                    if (Main.getGoodWorld)
                        velocity *= 2f;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MoltenBlob>(), BlobDamage, 0f, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 18f, targetHitbox);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if (info.Damage <= 0 || target.creativeGodMode)
                return;

            ProvUtils.ApplyDebuffs(target, 120);
        }
    }
}
