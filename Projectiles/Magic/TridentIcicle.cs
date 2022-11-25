using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class TridentIcicle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.Bullet;
            Projectile.friendly = true;
            Projectile.coldDamage = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trident Icicle");
        }

        public override void AI()
        {
            //make pretty dust
            int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 88);
            Main.dust[index2].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int index2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 88);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
            }
        }
    }
}
