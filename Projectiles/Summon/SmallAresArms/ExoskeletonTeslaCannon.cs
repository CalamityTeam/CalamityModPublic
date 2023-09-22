using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Weapons.Summon;
using System.IO;

namespace CalamityMod.Projectiles.Summon.SmallAresArms
{
    public class ExoskeletonTeslaCannon : ExoskeletonCannon
    {
        public ref float TeslaOrbIndex => ref Projectile.localAI[0];

        public override int ShootRate => AresExoskeleton.TeslaCannonShootRate;

        public override float ShootSpeed => 16f;

        public override bool UsesSuperpredictiveness => true;

        public override Vector2 OwnerRestingOffset => HoverOffsetTable[HoverOffsetIndex];

        public override void ClampFirstLimbRotation(ref double limbRotation) => limbRotation = RotationalClampTable[HoverOffsetIndex];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(TeslaOrbIndex);

        public override void ReceiveExtraAI(BinaryReader reader) => TeslaOrbIndex = reader.ReadSingle();

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

            int damage = (int)(Projectile.damage * AresExoskeleton.TeslaOrbDamageFactor);
            Vector2 teslaOrbVelocity = shootDirection * ShootSpeed;
            Projectile teslaOrb = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, teslaOrbVelocity, ModContent.ProjectileType<MinionTeslaOrb>(), damage, 0f, Projectile.owner);
            teslaOrb.ai[0] = TeslaOrbIndex++ % 6;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DefaultDrawCannon(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SmallAresArms/ExoskeletonTeslaCannonGlowmask").Value);
            return false;
        }
    }
}
