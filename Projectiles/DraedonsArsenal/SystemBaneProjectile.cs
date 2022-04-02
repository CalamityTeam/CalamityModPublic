using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SystemBaneProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/DraedonsArsenal/SystemBane";

        public SoundEffectInstance ShittyMicrowaveMemeSound = null;
        public float Time
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const int LightningFireRate = 60;
        public const int FieldLightningFireRate = 45;
        public const float FieldRadius = 360f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("System Bane");
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
            projectile.tileCollide = true;
            projectile.timeLeft = 480;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.StickToTiles(false, true);

            Time++;
            projectile.rotation = projectile.velocity.ToRotation();
            if (projectile.velocity.Y < 15f)
            {
                projectile.velocity.Y += 0.5f;
            }
            // Generate idle sparks.
            if (Time % 15f == 0f)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 229);
                dust.velocity = Main.rand.NextVector2Circular(10f, 10f);
                dust.fadeIn = 1.05f;
                dust.noGravity = true;
            }
            // Every so often, generate some lightning at a nearby enemy, if one exists.
            if (Time % LightningFireRate == 0f && Main.myPlayer == projectile.owner)
            {
                int lightningDamage = projectile.damage;
                int totalSystemBanes = Main.player[projectile.owner].ownedProjectileCounts[projectile.type];

                // Make the damage of the lightning have diminishing returns depending on how many systems are present.
                lightningDamage = (int)Math.Ceiling(lightningDamage / Math.Pow(totalSystemBanes, 1D / 3D));

                NPC potentialTarget = projectile.Center.ClosestNPCAt(900f);
                if (potentialTarget != null)
                    Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(potentialTarget.Center) * 15f, ModContent.ProjectileType<SystemBaneLightning>(), lightningDamage, projectile.knockBack, projectile.owner);
            }

            // Sometimes generate lightning from the outside of the energy field if the projectile was spawned by a stealth strike.
            if (projectile.Calamity().stealthStrike)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(FieldRadius);
                if (Time % FieldLightningFireRate == 0f && potentialTarget != null && Main.myPlayer == projectile.owner)
                {
                    Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2CircularEdge(FieldRadius, FieldRadius);
                    Projectile.NewProjectile(spawnPosition, potentialTarget.DirectionFrom(spawnPosition) * 14f, ModContent.ProjectileType<SystemBaneLightning>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }

            PlayMicrowaveSounds();
        }

        public void PlayMicrowaveSounds()
        {
            if (ShittyMicrowaveMemeSound is null && projectile.Calamity().stealthStrike)
            {
                ShittyMicrowaveMemeSound = ModContent.GetSound("CalamityMod/Sounds/Custom/MMMMMMMMMMMMM").CreateInstance();
                ShittyMicrowaveMemeSound.IsLooped = true;
                CalamityUtils.ApplySoundStats(ref ShittyMicrowaveMemeSound, projectile.Center);
                Main.PlaySoundInstance(ShittyMicrowaveMemeSound);
            }
            else if (ShittyMicrowaveMemeSound != null && !ShittyMicrowaveMemeSound.IsDisposed)
                CalamityUtils.ApplySoundStats(ref ShittyMicrowaveMemeSound, projectile.Center);
        }

        public override void Kill(int timeLeft)
        {
            ShittyMicrowaveMemeSound?.Stop();
            ShittyMicrowaveMemeSound?.Dispose();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!projectile.Calamity().stealthStrike)
                return;
            int totalCirclePoints = 55;
            float generalOpacity = Utils.InverseLerp(0f, 30f, projectile.timeLeft, true) * Utils.InverseLerp(480f, 450f, projectile.timeLeft, true);
            Texture2D lightningTexture = ModContent.GetTexture("CalamityMod/Projectiles/LightningProj");
            for (int i = 0; i < totalCirclePoints; i++)
            {
                float angle = MathHelper.TwoPi * i / totalCirclePoints;
                float nextAngle = angle + MathHelper.TwoPi / totalCirclePoints;
                float radiusOffset = (float)Math.Cos(Main.GlobalTime * 65f);
                Vector2 start = projectile.Center + angle.ToRotationVector2() * (FieldRadius + radiusOffset) - Main.screenPosition;
                Vector2 end = projectile.Center + nextAngle.ToRotationVector2() * (FieldRadius + radiusOffset) - Main.screenPosition;

                DelegateMethods.f_1 = SystemBaneLightning.InnerLightningOpacity * generalOpacity;
                DelegateMethods.c_1 = SystemBaneLightning.InnerLightningColor;
                Utils.DrawLaser(spriteBatch, lightningTexture, start, end, new Vector2(SystemBaneLightning.InnerLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));

                DelegateMethods.f_1 = SystemBaneLightning.OuterLightningOpacity * generalOpacity;
                DelegateMethods.c_1 = SystemBaneLightning.OuterLightningColor;
                Utils.DrawLaser(spriteBatch, lightningTexture, start, end, new Vector2(SystemBaneLightning.OuterLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity.X *= 0.8f;
            return false;
        }
    }
}

