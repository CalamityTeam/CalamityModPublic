using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class TridentIcicle : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.friendly = true;
            projectile.coldDamage = true;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.ignoreWater = true;
            projectile.coldDamage = true;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trident Icicle");
        }

        public override void AI()
        {
            //make pretty dust
            int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88);
            Main.dust[index2].noGravity = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 180);
            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item27, projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int index2 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 88);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 2f;
            }
        }
    }
}
