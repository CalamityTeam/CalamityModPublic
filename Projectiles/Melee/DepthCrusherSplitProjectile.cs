using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class DepthCrusherSplitProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int Time = 0;
        public int randTimer;
        public int dustType1 = 104;
        public int dustType2 = 96;
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Time++;
            if (Time == 1)
            {
                randTimer = Main.rand.Next(200, 261);
                Projectile.timeLeft = randTimer;
            }
            if (Time > 20 && Time < (randTimer - 70))
            {
                CalamityUtils.HomeInOnNPC(Projectile, true, 384f, MathHelper.Clamp(1f + Time * 0.12f, 1, 11), 20f);
            }
            else if (Time >= (randTimer - 70))
            {
                if (Projectile.velocity.Y < 10)
                    Projectile.velocity.Y += 0.4f;
                Projectile.velocity.X *= 0.97f;
            }
            if (Time % 2 == 0)
            {
                Color smokeColor = Color.MediumBlue;
                Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(-0.2f, -0.6f), smokeColor, 30, Main.rand.NextFloat(0.35f, 0.5f), 0.3f, Main.rand.NextFloat(-0.2f, 0.2f), false, required: true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
            for (int i = 0; i < 3; i++)
            {
                Vector2 dustPos = Projectile.Center;
                int dustType = Main.rand.NextBool(3) ? dustType1 : dustType2;
                Dust dust = Dust.NewDustPerfect(dustPos, dustType);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
                dust.velocity = new Vector2(0.5f, 0.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.1f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 180);

            SoundEngine.PlaySound(SoundID.ShimmerWeak1 with { Pitch = 0.35f }, Projectile.Center);
        }
    }
}
