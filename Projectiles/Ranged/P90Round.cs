using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Ranged
{
    public class P90Round : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 5;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
            Projectile.tileCollide = true;
            Projectile.ArmorPenetration = 15;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 598)
            {
                for (int i = 0; i <= 3; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 303 : 244, (Projectile.velocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.2f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                }
            }
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Vector3 DustLight = new Vector3(0.190f, 0.190f, 0.190f);
            Lighting.AddLight(Projectile.Center, DustLight * 2);

            if (targetDist < 1400f && Projectile.timeLeft < 596 && Projectile.timeLeft % 2 == 0)
            {
                int positionVariation = Projectile.timeLeft < 565 ? 25 : Projectile.timeLeft < 585 ? 12 : 5;
                LineParticle spark = new LineParticle(Projectile.Center - Projectile.velocity * 0.75f + Main.rand.NextVector2Circular(positionVariation, positionVariation), -Projectile.velocity * Main.rand.NextFloat(0.003f, 0.001f), false, 4, 1.45f, Color.Chocolate);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GenericSparkle sparker = new GenericSparkle(Projectile.Center + Projectile.velocity.RotatedByRandom(0.3f), Vector2.Zero, Color.White, Color.Chocolate, Main.rand.NextFloat(0.7f, 1.5f), Main.rand.Next(9, 17), Main.rand.NextFloat(-0.01f, 0.01f), 2.5f);
            GeneralParticleHandler.SpawnParticle(sparker);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 1.5f + Main.rand.NextVector2Circular(9, 9), Main.rand.NextBool(3) ? 303 : 244, (-Projectile.velocity * Main.rand.NextFloat(0.2f, 3f)).RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default);
                dust.noGravity = true;
                dust.scale = dust.type == 244 ? Main.rand.NextFloat(1.8f, 2.5f) : Main.rand.NextFloat(1.4f, 1.8f);
                dust.fadeIn = dust.type == 244 ? 1.2f : 0;
            }
        }
    }
}
