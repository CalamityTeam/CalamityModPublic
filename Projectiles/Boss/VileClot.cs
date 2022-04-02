using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class VileClot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Clot");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.light = 0.6f;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlaySound(SoundID.Item20, projectile.position);
            }

            int num104 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y),
                projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f, 100, default, 1.5f);
            Main.dust[num104].noGravity = true;

            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.CursedInferno, 60);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item10, projectile.position);
            for (int num584 = 0; num584 < 6; num584++)
            {
                int num585 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 2.5f);
                Main.dust[num585].noGravity = true;
                Dust dust = Main.dust[num585];
                dust.velocity *= 2f;
                num585 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, -projectile.velocity.X * 0.2f, -projectile.velocity.Y * 0.2f, 100, default, 1.2f);
                dust = Main.dust[num585];
                dust.velocity *= 2f;
            }
        }
    }
}
