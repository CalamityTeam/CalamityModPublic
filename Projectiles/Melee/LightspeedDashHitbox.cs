using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class LightspeedDashHitbox : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 75;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 18;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = Projectile.width * Projectile.scale;
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override void AI()
        {
            // Go where the player is going
            Projectile.Center = Main.player[Projectile.owner].Center;
            Main.player[Projectile.owner].velocity = Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld() + MathHelper.Pi.ToRotationVector2());
            SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { Volume = CommonCalamitySounds.SwiftSliceSound.Volume * 0.33f }, Projectile.Center);

            var player = Main.player[Projectile.owner];
            var modPlayer = player.GetModPlayer<LightspeedPlayer>();

            // Refund 30 energy if dash hits. Can get energy from multiple enemies.
            modPlayer.elementalMastery += 30;
            modPlayer.elementalMastery = Math.Min(modPlayer.elementalMastery, Lightspeed.MaxEnergy);

            // On-hit cut FX
            int points = 2;
            float radians = MathHelper.TwoPi / points;
            Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f)).RotatedByRandom(100);

            for (int k = 0; k < points; k++)
            {
                Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);

                Particle spark = new RainbowGlowSparkParticle((target.Center + velocity * 7.5f), velocity * 0.5f, false, 14, 0.07f, Color.Aqua, new Vector2(0.55f, 0.825f), true, true, hueShift: 0.05f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i < 12; i++)
            {
                Vector2 particleSpeed = Utils.SafeNormalize(target.Center, Vector2.One).RotatedByRandom(MathHelper.TwoPi * 3) * Main.rand.NextFloat(4f, 8f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.4f, 0.9f), Color.OrangeRed, 50, 2, 2.5f, hueShift: 0.06f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }

            target.AddBuff(ModContent.BuffType<ElementalMix>(), 300);
        }
    }
}
