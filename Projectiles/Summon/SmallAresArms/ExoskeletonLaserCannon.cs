using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonLaserCannon : ExoskeletonCannon
    {
        public ref float ShootCounter => ref Projectile.ai[0];

        // This is how many small, regular lasers will happen before the laser will charge up and prepare a laserbeam.
        public const int NormalLasersBeforeBeam = 6;

        public override Vector2 ConnectOffset => new((OwnerRestingOffset.X > 0f).ToDirectionInt() * 14f, -30f);

        public override int ShootRate => ShootCounter % NormalLasersBeforeBeam == 0f ? 150 : 36;

        public override float ShootSpeed => 13.5f;

        public override Vector2 OwnerRestingOffset => new(190f, -102f);

        public override void ClampFirstLimbRotation(ref double limbRotation)
        {
            limbRotation = -0.2f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Cannon");
            Main.projFrames[Type] = 6;
        }

        public override void ShootAtTarget(NPC target, Vector2 shootDirection)
        {
            // Play the laser cannon fire sound.
            SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = 0.4f }, Projectile.Center);

            // Create a burst of dust.
            for (int i = 0; i < 24; i++)
            {
                Dust laserEnergy = Dust.NewDustPerfect(Projectile.Center + shootDirection * Projectile.width * Projectile.scale * 0.45f, 182);
                laserEnergy.velocity = (MathHelper.TwoPi * i / 24f).ToRotationVector2() * 4f;
                laserEnergy.scale = 1.1f;
                laserEnergy.fadeIn = 0.4f;
                laserEnergy.noGravity = true;
            }

            // Shoot the laser. This only happens for the owner client.
            if (Main.myPlayer != Projectile.owner)
                return;

            int laserID = ModContent.ProjectileType<MinionLaserBurst>();
            bool fireLaser = ShootCounter % NormalLasersBeforeBeam == NormalLasersBeforeBeam - 1f;
            if (fireLaser)
                laserID = ModContent.ProjectileType<CannonLaserbeam>();

            Vector2 laserVelocity = shootDirection * ShootSpeed;
            int laser = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, laserVelocity, laserID, Projectile.damage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(laser))
            {
                Main.projectile[laser].originalDamage = Projectile.originalDamage;
                if (fireLaser)
                    Main.projectile[laser].ai[1] = Projectile.whoAmI;
            }

            // Increment the shoot counter. This decides if the cannon fires a simple laser or a beam.
            ShootCounter++;
            Projectile.netUpdate = true;
        }

        // Create some charge dust before firing a laserbeam.
        public override void PostAI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
            if (ShootCounter % NormalLasersBeforeBeam != NormalLasersBeforeBeam - 1f || !TargetingSomething)
                return;

            Vector2 aimDirection = Projectile.rotation.ToRotationVector2();
            Dust laserEnergy = Dust.NewDustPerfect(Projectile.Center + aimDirection * Projectile.width * Projectile.scale * 0.51f, 182);
            laserEnergy.velocity = aimDirection.RotatedByRandom(0.48f) * Main.rand.NextFloat(3f);
            laserEnergy.scale = 1.1f;
            laserEnergy.fadeIn = 0.4f;
            laserEnergy.noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonLaserCannonGlowmask").Value;
            Rectangle frame = texture.Frame(2, Main.projFrames[Type], TargetingSomething.ToInt(), Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = Projectile.rotation;
            if (Projectile.spriteDirection == -1)
                rotation += MathHelper.Pi;

            DrawLimbs();
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), rotation, origin, Projectile.scale, direction, 0);

            return false;
        }
    }
}
