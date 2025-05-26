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
    public class NeptunesBountySplitProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int Time = 0;
        public int randTimer;
        public int dustType1 = 80;
        public int dustType2 = 172;
        public int spreadDust = 0;
        public Color WaterColor = Main.rand.NextBool() ? Color.DodgerBlue : Color.DeepSkyBlue;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 110;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            Time++;
            Projectile.velocity *= 0.988f;

            if (Projectile.timeLeft % 2 == 0 && Time > 3f && playerDist < 1400f)
            {
                Color smokeColor = Color.RoyalBlue;
                Particle smoke = new HeavySmokeParticle(Projectile.Center - Projectile.velocity, Projectile.velocity * Main.rand.NextFloat(-0.2f, -0.6f), smokeColor * 0.65f, 15, Main.rand.NextFloat(0.4f, 0.55f), 0.3f, Main.rand.NextFloat(-0.2f, 0.2f), false, required: true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(3 + spreadDust, 3 + spreadDust), Main.rand.NextBool(3) ? dustType1 : dustType2, -Projectile.velocity * Main.rand.NextFloat(0.05f, 0.65f), 0, default, Main.rand.NextFloat(0.4f, 1.2f) + Time * 0.009f);
            dust.noGravity = true;
            if (dust.type == dustType1)
                dust.alpha = 180;

            if (Projectile.timeLeft == 20)
            {
                WaterFlavoredParticle spark = new WaterFlavoredParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 25, 0.85f + Time * 0.013f, WaterColor * 0.35f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            else if (Projectile.timeLeft > 20)
            {
                WaterFlavoredParticle spark = new WaterFlavoredParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 2, 0.85f + Time * 0.013f, WaterColor * 0.35f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Projectile.timeLeft < 20)
            {
                Time -= 5;
                spreadDust += 2;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 240);

            SoundEngine.PlaySound(SoundID.ShimmerWeak1 with { Pitch = 0.35f }, Projectile.Center);
        }
    }
}
