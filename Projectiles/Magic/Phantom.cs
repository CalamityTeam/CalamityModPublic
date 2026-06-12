using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Phantom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public float sizeVariance = 2;
        public int time = 0;
        public int spinDir = 100;
        public int waveOften = 40;
        public float scaleVariance = 1;
        public NPC targeted;

        public bool launched = false;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * Projectile.MaxUpdates;
            Projectile.ArmorPenetration = 30;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.rotation += Main.rand.NextFloat(0.02f, 0.09f);

            if (spinDir == 100)
            {
                spinDir = Main.rand.NextBool() ? 1 : -1;
                waveOften = Main.rand.Next(10, 40 + 1);
                Projectile.scale = Main.rand.NextFloat(0.95f, 1.1f);
            }

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.3f);

            if (time % 2 == 0 && time > 3 && targetDist < 1400)
            {
                Particle spark = new GlowSparkParticle(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(-1, 1), -Projectile.velocity * 0.3f, false, 4, 0.025f, Color.Lerp(Color.White, Color.Aqua, 0.3f) * 0.6f, new Vector2(1, 0.3f), true, false, 2);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (time >= 500)
            {
                Vector2 mouse = Owner.ClampedMouseWorld();
                if (time == 500)
                {
                    Projectile.penetrate = 1;
                    Projectile.velocity = (mouse - Projectile.Center).SafeNormalize(Vector2.UnitX) * 6;
                    launched = true;
                }

                float badDist = Vector2.Distance(mouse, Projectile.Center);
                if (badDist < 30)
                {
                    time = 600;
                }
                if (targeted == null || targeted.life <= 0)
                    targeted = Projectile.Center.ClosestNPCAt(950);
                CalamityUtils.HomeInOnSelectedNPC(Projectile, targeted, true, 0.15f, 6, 0.98f, accelerate: true);

                if (time < 550 && targeted == null)
                {
                    if (Projectile.velocity.Length() < 6)
                        Projectile.velocity += (mouse - Projectile.Center).SafeNormalize(Vector2.UnitX) * 0.35f;
                    else
                        Projectile.velocity *= 0.9f;
                }

                if (time % waveOften == 0)
                    spinDir *= -1;

                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(0.01f, 0.02f) * spinDir);
            }
            else if (time > 15)
            {
                Vector2 circle = Owner.Center + new Vector2(0, -30).RotatedBy(time * 0.05f);
                Vector2 moveToEnemy = (circle - Projectile.Center).SafeNormalize(Vector2.UnitX);
                if (Projectile.velocity.Length() < 8)
                    Projectile.velocity += moveToEnemy * Main.rand.NextFloat(0.2f, 0.4f);
                else
                    Projectile.velocity *= 0.85f;
            }

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= (launched ? 1f : 0.3f);

            Player Owner = Main.player[Projectile.owner];

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, target.Center);
            target.MoveNPC(launchVel, 10 * (launched ? 0.5f : 1), true, Owner);
            
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (targeted != null)
                return (target == targeted ? null : false);
            else
                return null;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 2; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.SpectreStaff, (Projectile.velocity * 3).RotatedByRandom(0.6f) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(0.8f, 1.3f));
                dust.noGravity = true;
            }
        }
    }
}
