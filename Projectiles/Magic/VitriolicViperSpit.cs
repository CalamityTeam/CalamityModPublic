using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Magic
{
    public class VitriolicViperSpit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spit");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, 10, 10, 27);
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 60);
        }
    }
}
