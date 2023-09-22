using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace CalamityMod.Projectiles.Summon
{
    public class TundraFlameBlossomsOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];

        public ref float TimerForCharging => ref Projectile.ai[0];

        public ref float TypeOfFlowerOrb => ref Projectile.ai[1];
        
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            TypeOfFlowerOrb = Main.rand.Next(2);
            TypeOfFlowerOrb = (TypeOfFlowerOrb == 0f) ? 6 : 113;
            // DustID 6 is torch dust, DustID 113 is Frost Blossom's dust.

            SpawnDust(); // Makes a dust effect when spawned.
            DustTrail(); // When spawned, it'll actively make a dust effect correspondent to their type.
            TargetNPC(); // Homes slowly on the target.

            Lighting.AddLight(Projectile.Center, Color.White.ToVector3());
            Projectile.rotation += MathHelper.ToRadians(Projectile.velocity.X);
        }

        #region Methods

        public void SpawnDust()
        {
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 45; i++) // Spawns a dust circle with the correspondent color.
                {
                    float angle = MathHelper.TwoPi / 45f * i;
                    Vector2 velocity = angle.ToRotationVector2() * 4f;
                    Dust circleSpawn = Dust.NewDustPerfect(Projectile.Center + velocity * 3.5f, (int)TypeOfFlowerOrb, velocity, 0, default, 1.25f);
                    circleSpawn.noGravity = true;
                }
            }
            Projectile.localAI[0] = 1f;
        }

        public void DustTrail()
        {
            Dust dustTrail = Dust.NewDustPerfect(Projectile.Center, (int)TypeOfFlowerOrb);
            dustTrail.velocity = Main.rand.NextVector2Circular(2f, 2f);
            dustTrail.noGravity = true;
            dustTrail.scale = 2f;
            Projectile.netUpdate = true;
        }

        public void TargetNPC() // Starts simply going away from the owner and slowly homes towards the target.
        {
            NPC potentialTarget = Projectile.Center.MinionHoming(1700f, Owner);
            if (potentialTarget != null)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, (potentialTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 30f, TimerForCharging);
                TimerForCharging += 0.001f;
                TimerForCharging = (TimerForCharging > 1f) ? 1f : TimerForCharging;
            }
            Projectile.netUpdate= true;
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            DustFlowerOnHit(); // Makes a dust effect that looks like a flower.

            target.AddBuff(BuffID.OnFire3, 240);
            target.AddBuff(BuffID.Frostburn2, 240);
        }

        public void DustFlowerOnHit()
            // Copied code from Frost Beam, from the Frost Blossom Staff.
            // Modified to have a different color correspondent on the type of flower.
        {
            int flowerPetalCount = Main.rand.Next(3, 5 + 1);
            float thetaDelta = Projectile.velocity.ToRotation();
            float weaveDistanceMin = 2f;
            float weaveDistanceOutwardMax = 3f;
            float weaveDistanceInner = 0.5f;
            for (float theta = 0f; theta < MathHelper.TwoPi; theta += 0.05f)
            {
                Vector2 velocity = theta.ToRotationVector2() *
                    (weaveDistanceMin +
                    // The 0.5 in here is to prevent the petal from looping back into itself. With a 0.5 addition, it is perfect, coming back to (0,0)
                    // instead of weaving backwards.
                    (float)(Math.Sin(thetaDelta + theta * flowerPetalCount) + 0.5f + weaveDistanceInner) *
                    weaveDistanceOutwardMax);
                Dust dustFlower = Dust.NewDustPerfect(Projectile.Center, (int)TypeOfFlowerOrb, velocity, 0, default, 1.25f);
                dustFlower.noGravity = true;
                dustFlower.scale = 1.35f;
            }
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        #endregion
    }
}
