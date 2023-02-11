using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class UmbraphileBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbra Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void Kill(int timeLeft)
        {
            for (int s = 0; s < 20; s++)
            {
                int dustType = Main.rand.NextBool() ? 246 : 176;
                float dustSpeed = Main.rand.NextFloat(3f, 9f);
                Vector2 dustVel = CalamityUtils.RandomVelocity(10f, dustSpeed, dustSpeed, 1f);
                int boom = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1f);
                Dust dust = Main.dust[boom];
                dust.noGravity = true;
                dust.position = Projectile.Center;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity = dustVel;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.dayTime || Main.rand.NextBool(3)) //100% during day, 33.33% chance at night
                target.AddBuff(BuffID.Daybreak, 45);

            if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
                target.AddBuff(ModContent.BuffType<Nightwither>(), 45);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.dayTime || Main.rand.NextBool(3)) //100% during day, 33.33% chance at night
                target.AddBuff(BuffID.Daybreak, 45);

            if (!Main.dayTime || Main.rand.NextBool(3)) //100% at night, 33.33% chance during day
                target.AddBuff(ModContent.BuffType<Nightwither>(), 45);
        }
    }
}
