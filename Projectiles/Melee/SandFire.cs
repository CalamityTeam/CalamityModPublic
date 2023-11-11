using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class SandFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 60;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0.25f, 0f);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int shiny = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 159, 0f, 0f, 100, default, 0.5f);
                    Main.dust[shiny].noGravity = true;
                    Main.dust[shiny].velocity *= 0f;
                }
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 159, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 120);
        }
    }
}
