using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class StellarTorusBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/StellarTorusBeamStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/StellarTorusBeamMiddle", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/StellarTorusBeamEnd", AssetRequestMode.ImmediateLoad).Value;

        public override float Lifetime => StellarTorusStaff.TimeShooting;
        public override float MaxLaserLength => StellarTorusStaff.EnemyDetectionDistance * 2f;
        public override float MaxScale => 1f;

        public float Opacity = .33f;
        public override Color LightCastColor => Color.White * Opacity;
        public override Color LaserOverlayColor => Color.White * Opacity;

        public ref float MinionID => ref Projectile.ai[1];
        public Projectile Minion => Main.projectile[(int)MinionID];
        public ref float TargetID => ref Projectile.ai[2];
        public NPC Target => Main.npc[(int)TargetID];

        public override void SetStaticDefaults() => ProjectileID.Sets.MinionShot[Type] = true;

        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = StellarTorusStaff.IFrames;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 20;
            Projectile.alpha = 254;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void ExtraBehavior()
        {
            Projectile.velocity = Minion.rotation.ToRotationVector2();

            if (Target is null)
                Projectile.Kill();

            if (Main.rand.NextBool(3))
            {
                SparkParticle energyEffect = new SparkParticle(Projectile.Center + Vector2.Lerp(Projectile.velocity, Projectile.velocity * MaxLaserLength, Main.rand.NextFloat(1f)) + Main.rand.NextVector2Circular(30f, 30f),
                    Projectile.velocity * Main.rand.NextFloat(8f, 12f),
                    false,
                    20,
                    Main.rand.NextFloat(1f, 1.5f),
                    Color.Cyan);
                GeneralParticleHandler.SpawnParticle(energyEffect);
            }

            GenericBloom bloomEnergyEffect = new GenericBloom(Projectile.Center,
                Minion.velocity,
                Color.Cyan with { A = 6 },
                0.6f,
                (int)(Lifetime / 4f));
            GeneralParticleHandler.SpawnParticle(bloomEnergyEffect);
        }

        public override void AttachToSomething()
        {
            Projectile.Center = Minion.Center;
            if (Minion is null)
            {
                Projectile.Kill();
                Projectile.netUpdate = true;
            }
        }
    }
}
