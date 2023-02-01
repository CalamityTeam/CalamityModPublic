using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class ThanatosBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults() => DisplayName.SetDefault("Boom");

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 45;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                CreateExplosionDust();
                Projectile.localAI[0] = 1f;
            }
        }

        public void CreateExplosionDust()
        {
            if (Main.dedServ)
                return;

            for (float speed = 2f; speed <= 6f; speed += 0.7f)
            {
                float lifePersistance = Main.rand.NextFloat(0.8f, 1.7f);
                for (int i = 0; i < 60; i++)
                {
                    Dust energy = Dust.NewDustPerfect(Projectile.Center, 267);
                    energy.velocity = (MathHelper.TwoPi * i / 60f).ToRotationVector2() * speed;
                    energy.noGravity = true;
                    energy.color = Main.hslToRgb(Main.rand.NextFloat(), 0.7f, 0.625f);
                    energy.fadeIn = lifePersistance;
                    energy.scale = 1.4f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
    }
}
