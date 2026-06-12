using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistExceptionAttribute(true)]
    public class LightspeedM1Hitbox : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];

        public bool gotEnergyThisSwing = false;

        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 6;
            Projectile.DamageType = TrueMeleeNoSpeedDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 4;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            float hitboxSize = Projectile.width * Projectile.scale;
            Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10;
            Vector2 start = (Projectile.timeLeft == 6 ? Main.player[Projectile.owner].Center : Projectile.Center - vel);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, Projectile.Center + vel, hitboxSize * Projectile.scale, ref _);

        }
        public override void AI()
        {
            Vector2 toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld() + MathHelper.Pi.ToRotationVector2());

            float rotation = toMouse.ToRotation();

            Projectile.rotation = rotation;
            Projectile.velocity = toMouse * 34 * Projectile.scale;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 toMouse = Utils.DirectionTo(Owner.Center, Owner.ClampedMouseWorld() + MathHelper.Pi.ToRotationVector2());

            if (!gotEnergyThisSwing)
            {
                gotEnergyThisSwing = true;
                var player = Main.player[Projectile.owner];
                var modPlayer = player.GetModPlayer<LightspeedPlayer>();

                // +4 energy on hit
                modPlayer.elementalMastery += 4;
                modPlayer.elementalMastery = Math.Min(modPlayer.elementalMastery, Lightspeed.MaxEnergy);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 particleSpeed = Utils.SafeNormalize(target.Center * toMouse, Vector2.One).RotatedByRandom(MathHelper.PiOver4 * 0.8f) * Main.rand.NextFloat(7f, 14f);
                Particle energyLeak = new SquishyLightParticle(target.Center, particleSpeed, Main.rand.NextFloat(0.25f, 0.5f), Color.OrangeRed, 22, 2, 2.5f, hueShift: 0.06f);
                GeneralParticleHandler.SpawnParticle(energyLeak);
            }

            target.AddBuff(ModContent.BuffType<ElementalMix>(), 60);
        }
    }
}
