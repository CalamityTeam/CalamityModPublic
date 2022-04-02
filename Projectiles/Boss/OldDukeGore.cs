using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class OldDukeGore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Gore");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.alpha = 255;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            projectile.alpha -= 50;
            if (projectile.alpha < 0)
                projectile.alpha = 0;

            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 15f)
            {
                projectile.velocity.Y += 0.1f;
            }

            if (projectile.velocity.Y > 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.tileCollide = projectile.timeLeft < 240;

            projectile.rotation += projectile.velocity.X * 0.1f;

            int blood = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Blood, 0f, 0f, 100, default, 1f);
            Main.dust[blood].noGravity = true;
            Main.dust[blood].velocity *= 0f;

            int acid = Dust.NewDust(projectile.position, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 1f);
            Main.dust[acid].noGravity = true;
            Main.dust[acid].velocity *= 0f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath12, projectile.position);

            int num226 = 8;
            for (int num227 = 0; num227 < num226; num227++)
            {
                Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                source = source.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                Vector2 dustVel = source - projectile.Center;
                int blood = Dust.NewDust(source + dustVel, 0, 0, DustID.Blood, dustVel.X, dustVel.Y, 100, default, 1.2f);
                Main.dust[blood].noGravity = true;
                Main.dust[blood].noLight = true;
                Main.dust[blood].velocity = dustVel;
            }

            for (int d = 0; d < 6; d++)
            {
                int acid = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 3f);
                Main.dust[acid].noGravity = true;
                Main.dust[acid].velocity *= 5f;
                acid = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default(Color), 2f);
                Main.dust[acid].velocity *= 2f;
                Main.dust[acid].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
