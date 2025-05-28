using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class SuperradiantSawLingering : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/SuperradiantSaw";

        public ref float Time => ref Projectile.ai[1];

        public Particle SmallSlashSmear;
        public Particle LargeSlashSmear;
        public static Asset<Texture2D> SawOutline;
        public static Asset<Texture2D> SmallSlash;
        public static Asset<Texture2D> LargeSlash;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 270;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            // Timer and rotation
            Time++;
            Projectile.rotation += MathHelper.ToRadians(42f);

            // Make it lose velocity as it travels
            Projectile.velocity *= 0.955f;

            // Continously spawn homing bolts and small saws
            if (Time % 12 == 0 && Time > 30)
            {
                Vector2 randVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7.5f, 9f);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, randVelocity, ModContent.ProjectileType<SuperradiantBolt>(), (int)(Projectile.damage * 0.5f), 0f, Main.myPlayer);
            }

            // Fade out at the end of its lifetime
            if (Projectile.timeLeft <= 30)
            {
                Projectile.alpha += 8;
                if (Projectile.alpha > 255)
                    Projectile.Kill();
            }

            // Rainbow smear particles which follow the path of the slashes
            if (LargeSlashSmear == null)
            {
                LargeSlashSmear = new CircularSmearVFX(Projectile.Center, Color.Black, Time * -Projectile.rotation, 1.35f);
                GeneralParticleHandler.SpawnParticle(LargeSlashSmear);
            }
            else
            {
                LargeSlashSmear.Rotation = -Projectile.rotation;
                LargeSlashSmear.Time = 0;
                LargeSlashSmear.Position = Projectile.Center;
                LargeSlashSmear.Scale = 1.35f;
                LargeSlashSmear.Color = Main.hslToRgb(0.5f + 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f), 1f, 0.6f) * 0.8f * Projectile.Opacity;
            }
            if (SmallSlashSmear == null)
            {
                SmallSlashSmear = new CircularSmearVFX(Projectile.Center, Color.Black, Projectile.rotation, 0.8f);
                GeneralParticleHandler.SpawnParticle(SmallSlashSmear);
            }
            else
            {
                SmallSlashSmear.Rotation = Projectile.rotation;
                SmallSlashSmear.Time = 0;
                SmallSlashSmear.Position = Projectile.Center;
                SmallSlashSmear.Scale = 0.8f;
                SmallSlashSmear.Color = Main.hslToRgb(0.5f + 0.5f * MathF.Cos(Main.GlobalTimeWrappedHourly * 5f), 1f, 0.6f) * 0.6f * Projectile.Opacity;
            }
        }

        public override bool? CanDamage() => Projectile.timeLeft > 30;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 180);
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 90);
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SwiftSlice"), Projectile.Center);

            // SUPER EPIC AND AWESOME PARTICLES
            for (int s = 0; s < 12; s++)
            {
                Vector2 sparkVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(14f, 18f);
                float sparkSize = Main.rand.NextFloat(1f, 1.4f);
                Color sparkColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);

                Particle sparked = new AltLineParticle(target.Center, sparkVel, false, 30, sparkSize, sparkColor);
                GeneralParticleHandler.SpawnParticle(sparked);
            }
            for (int sq = 0; sq < 5; sq++)
            {
                Vector2 squareVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(10f, 16f);
                float squareSize = Main.rand.NextFloat(3.2f, 4f);
                Color squareColor = Main.hslToRgb(Main.rand.NextFloat(), 0.6f, 0.8f);

                Particle squared = new SquareParticle(target.Center, squareVel, true, 30, squareSize, squareColor);
                GeneralParticleHandler.SpawnParticle(squared);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/CeramicImpact", 2), Projectile.Center);

            for (int i = 0; i < 32; i++)
            {
                Vector2 velocity = ((MathHelper.TwoPi * i / 32f) - (MathHelper.Pi / 32f)).ToRotationVector2() * 32f;
                Particle sparkle = new CritSpark(Projectile.Center, velocity, Color.White, Color.Lime, 1.5f, 30, 0.1f, 3f, Main.rand.NextFloat(0f, 0.01f));
                GeneralParticleHandler.SpawnParticle(sparkle);
            }
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox) => hitbox.Inflate(70, 70);

        public override bool PreDraw(ref Color lightColor)
        {
            LargeSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSawLargeSlash");
            Texture2D largeSlashTexture = LargeSlash.Value;
            SmallSlash ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSawSmallSlash");
            Texture2D smallSlashTexture = SmallSlash.Value;
            Color slashColor = new Color(200, 200, 200, 100) * Projectile.Opacity;

            Main.EntitySpriteDraw(largeSlashTexture, Projectile.Center - Main.screenPosition, null, slashColor, -Projectile.rotation, largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

            if (Time % 4 == 0)
            {
                Vector2 randomParticleOffset = new Vector2(Main.rand.NextFloat(-Projectile.width * 1.75f, Projectile.width * 1.75f), Main.rand.NextFloat(-Projectile.width * 1.75f, Projectile.width * 1.75f));
                float randomParticleScale = Main.rand.NextFloat(0.65f, 0.95f);
                Color bloomColor = Color.Lerp(new Color(29, 120, 30), new Color(56, 255, 59), MathF.Abs(MathF.Sin(Time)));
                Particle bloomCircle = new BloomParticle(Projectile.Center + randomParticleOffset, Projectile.velocity, Main.rand.NextBool() ? Color.White : bloomColor, randomParticleScale, randomParticleScale, 4, false);
                GeneralParticleHandler.SpawnParticle(bloomCircle);
            }
            Main.EntitySpriteDraw(smallSlashTexture, Projectile.Center - Main.screenPosition, null, slashColor, Projectile.rotation, smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);

            if (Time % 4 == 0)
            {
                Vector2 randomParticleOffset = new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), Main.rand.NextFloat(-Projectile.width, Projectile.width));
                float randomParticleScale = Main.rand.NextFloat(0.35f, 0.65f);
                Color bloomColor = Color.Lerp(new Color(29, 120, 30), new Color(56, 255, 59), MathF.Abs(MathF.Sin(Time)));
                Particle bloomCircle = new BloomParticle(Projectile.Center + randomParticleOffset, Projectile.velocity, Main.rand.NextBool() ? Color.White : bloomColor, randomParticleScale, randomParticleScale, 4, false);
                GeneralParticleHandler.SpawnParticle(bloomCircle);
            }

            // Draw the saw itself at full brightness, glow and outline in rainbow
            Texture2D buzzsawTexture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(buzzsawTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, buzzsawTexture.Size() * 0.5f, 1f, SpriteEffects.None);

            SawOutline ??= ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/SuperradiantSawOutline");
            Texture2D outline = SawOutline.Value;
            Main.EntitySpriteDraw(outline, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation, outline.Size() * 0.5f, 1f, SpriteEffects.None);

            if (!CalamityConfig.Instance.Afterimages)
                return false;

            // Special afterimage drawing to include the slashes
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                float afterimageRot = Projectile.oldRot[i];
                Vector2 drawPos = Projectile.oldPos[i] + buzzsawTexture.Size() * 0.5f - Main.screenPosition;
                float intensity = MathHelper.Lerp(0.1f, 0.6f, 1f - i / (float)Projectile.oldPos.Length);

                Main.EntitySpriteDraw(buzzsawTexture, drawPos, null, Color.White * intensity, afterimageRot, buzzsawTexture.Size() * 0.5f, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(largeSlashTexture, drawPos, null, slashColor * intensity, -afterimageRot, largeSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);
                Main.EntitySpriteDraw(smallSlashTexture, drawPos, null, slashColor * intensity, afterimageRot, smallSlashTexture.Size() * 0.5f, 1f, SpriteEffects.None);
            }
            return false;
        }
    }
}
