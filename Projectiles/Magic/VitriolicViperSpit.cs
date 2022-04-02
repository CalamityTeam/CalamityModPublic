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
            projectile.width = 16;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust dust = Dust.NewDustDirect(projectile.position, 10, 10, 27);
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
