using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Cinder : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public Player Owner => Main.player[Projectile.owner];
        
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;

            Projectile.width = 12;
            Projectile.height = 12;

            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesIDStaticNPCImmunity = true;
        }

        public override void AI()
        {
            SpawnDust();
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3());
        }

        public void SpawnDust()
        {
            int bootlegTexture = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1f);
            Main.dust[bootlegTexture].position += new Vector2(2f);
            Main.dust[bootlegTexture].scale += 0.3f + Main.rand.NextFloat(0.5f);
            Main.dust[bootlegTexture].noGravity = true;
            Main.dust[bootlegTexture].velocity.Y -= 2f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
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
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, velocity);
                dust.noGravity = true;
                dust.scale = 1.35f;
            }
            // Makes an orange flower made out of dust when it hits a target.

            target.AddBuff(BuffID.OnFire, 180);
        }
    }
}
