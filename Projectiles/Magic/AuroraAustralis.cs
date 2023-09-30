using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AuroraAustralis : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static float CosFrequency = 0.05f;
        private static float CosAmplitude = 0.008f;
        public int[] dustTypes = new int[]
        {
            ModContent.DustType<AstralBlue>(),
            ModContent.DustType<AstralOrange>()
        };

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.alpha = 255;

            CosFrequency = 0.15f;
            CosAmplitude = 0.06f;
        }

        public override void AI()
        {
            // On-spawn effects
            if (Projectile.ai[0] == 0)
            {
                // Store the X and Y of the spawn velocity so it can be used for trig calculations
                Projectile.localAI[0] = Projectile.velocity.X;
                Projectile.localAI[1] = Projectile.velocity.Y;
            }

            // Doesn't collide with tiles for the first 2 frames
            Projectile.tileCollide = Projectile.ai[0] > 2f;

            // Apply fancy cosine movement, then slight gravity, then cap velocity.
            // Original velocity is reconstructed so that it can be used in the calculation
            Vector2 originalVelocity = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
            ApplyCosVelocity(originalVelocity);
            float currentSpeed = Projectile.velocity.Length();
            float maxSpeed = 1.4f * originalVelocity.Length();
            if (currentSpeed > maxSpeed)
                Projectile.velocity *= maxSpeed / currentSpeed;

            // spawn dust
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.Next(dustTypes), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            int rainbow = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            Dust dust = Main.dust[rainbow];
            dust.velocity *= 0.1f;
            dust.velocity += Projectile.velocity * 0.2f;
            dust.position.X = Projectile.Center.X + 4f + Main.rand.Next(-2, 3);
            dust.position.Y = Projectile.Center.Y + Main.rand.Next(-2, 3);
            dust.noGravity = true;

            if (Projectile.timeLeft % 10 == 0 && Main.myPlayer == Projectile.owner) //spawn stars every 10 ticks
            {
                var source = Projectile.GetSource_FromThis();
                if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<AstralStarMagic>()] < 30)
                {
                    float dmgKBMult = Main.rand.NextFloat(0.25f, 0.75f);
                    Projectile star = CalamityUtils.ProjectileRain(source, Projectile.Center, Projectile.velocity.X, 100f, 500f, 800f, Main.rand.NextFloat(10f, 20f), ModContent.ProjectileType<AstralStarMagic>(), (int)(Projectile.damage * dmgKBMult), Projectile.knockBack * dmgKBMult, Projectile.owner);
                    star.timeLeft = 120;
                    star.ai[0] = 1f;
                }
            }

            Projectile.ai[0]++;
        }

        private void ApplyCosVelocity(Vector2 baseVelocity)
        {
            float radians = -(-MathHelper.PiOver2 + CosFrequency * Projectile.ai[0]);
            Projectile.velocity += CosAmplitude * baseVelocity.RotatedBy(radians);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, Main.rand.Next(dustTypes), Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);
        }
    }
}
