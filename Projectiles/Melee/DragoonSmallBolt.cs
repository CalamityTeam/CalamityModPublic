using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class DragoonSmallBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public float colorValue = 0; // The color fade from the first to second color
        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            colorValue = MathHelper.Lerp(colorValue, 50, 0.025f);
            Color usedColor = Color.Lerp(Color.Cyan, Color.Orchid, Utils.GetLerpValue(0, 50, colorValue)) * 0.7f;

            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] * 0.022f); // Add some outward arc to the movement

            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (targetDist < 1400f)
            {
                Vector2 pos = Projectile.Center;
                if (Projectile.timeLeft % 3 == 0)
                {
                    Particle spark2 = new BoltParticle(pos, -Projectile.velocity * 0.05f, false, 15, 0.4f * Projectile.scale, usedColor, new Vector2(1.8f, 0.8f), true, true, false, 0.5f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Main.rand.NextBool(35))
                {
                    Particle spark2 = new BoltParticle(pos, Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.9f), false, 23, Main.rand.NextFloat(0.2f, 0.25f) * Projectile.scale, usedColor, new Vector2(1.8f, 0.8f), true, true, false, 0.3f);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (time % 5 == 0)
                {
                    Dust dust = Dust.NewDustPerfect(pos, DustID.FireworksRGB, new Vector2(2, 2).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1f), 0, default, Main.rand.NextFloat(0.45f, 0.6f));
                    dust.noGravity = true;
                    dust.color = usedColor;
                }
            }
            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float minMult = 0.5f;
            int hitsToMinMult = 2; // Can only hit thrice
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);
            for (int i = 0; i < 4; i++)
            {
                Particle spark2 = new BoltParticle(Projectile.Center, Projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.9f), false, 13, Main.rand.NextFloat(0.3f, 0.35f), Main.rand.NextBool() ? Color.Orchid : Color.Cyan, new Vector2(1.8f, 0.8f), true, true, false, 0.9f);
                GeneralParticleHandler.SpawnParticle(spark2);
            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Owner = Main.player[Projectile.owner];
            if (time <= 1)
            {
                float _ = float.NaN;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Owner.Center, 25 * Projectile.scale, ref _);
            }
            else
                return CalamityUtils.CircularHitboxCollision(Projectile.Center, 25 * Projectile.scale, targetHitbox);
        }
        public override bool? CanCutTiles() => false;
    }
}
