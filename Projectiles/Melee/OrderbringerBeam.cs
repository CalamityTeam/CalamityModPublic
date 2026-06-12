using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OrderbringerBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/OrderbringerBeam";
        public int time = 0;
        public Color mainColor = Color.White;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 40;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
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
            Player player = Main.player[Projectile.owner];
            mainColor = player.Calamity().lightRGB;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);
            if (Projectile.timeLeft % 12 == 0 && targetDist < 1400f)
            {
                Particle spark = new CustomSpark(Projectile.Center, Projectile.velocity * 0.2f, "CalamityMod/Particles/BloomCircle", false, 23, 0.25f * Projectile.scale, mainColor * 0.75f, new Vector2(1f, 7.35f), true, true, shrinkSpeed: 0.2f, glowOpacity: 0.7f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], Color.Lerp(mainColor, Color.White, 0.3f) with { A = 0 }, 1, null, true, true);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 180);
            if (damageDone > 2 && !target.Calamity().IsArmored())
                Projectile.damage = (int)(Projectile.damage * 0.8f);
            if (Projectile.numHits < 1)
            {
                Particle orb = new GlowSparkParticle(target.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY), false, 12, 0.07f, mainColor, new Vector2(1.5f, 0.8f), true);
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }
    }
}
