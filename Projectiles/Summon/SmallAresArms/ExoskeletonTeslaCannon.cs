using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Summon;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonTeslaCannon : ExoskeletonCannon
    {
        public ref float TeslaOrbIndex => ref Projectile.ai[0];

        public override int ShootRate => AresExoskeleton.TeslaCannonShootRate;

        public override float ShootSpeed => 12f;

        public override bool UsesSuperpredictiveness => true;

        public override Vector2 OwnerRestingOffset => new(-300f, 96f);

        public override void ClampFirstLimbRotation(ref double limbRotation)
        {
            limbRotation = MathHelper.Pi - 0.23f;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tesla Cannon");
            Main.projFrames[Type] = 6;
        }

        public override void PostAI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];
        }

        public override void ShootAtTarget(NPC target, Vector2 shootDirection)
        {
            // Play the plasma bolt fire sound.
            SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBoltSound with { Volume = 0.4f }, Projectile.Center);
            
            // Shoot the tesla orb. This only happens for the owner client.
            if (Main.myPlayer != Projectile.owner)
                return;

            Vector2 teslaOrbVelocity = shootDirection * ShootSpeed;
            int teslaOrb = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, teslaOrbVelocity, ModContent.ProjectileType<MinionTeslaOrb>(), Projectile.damage, 0f, Projectile.owner);
            if (Main.projectile.IndexInRange(teslaOrb))
            {
                Main.projectile[teslaOrb].originalDamage = Projectile.originalDamage;
                Main.projectile[teslaOrb].ai[0] = TeslaOrbIndex++ % 6;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DefaultDrawCannon(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonTeslaCannonGlowmask").Value);
            return false;
        }
    }
}
