using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlanarRipperBolt : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/ShockGrenadeBolt";

        public static int frameWidth = 12;
        public static int frameHeight = 26;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bolt");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.extraUpdates = 10;
            projectile.timeLeft = 600;
            projectile.ranged = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            target.AddBuff(BuffID.Electrified, 180);
            if (projectile.owner == Main.myPlayer)
            {
                if (target.life <= 0)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                if (crit)
                {
                    if (modPlayer.planarSpeedBoost < 20)
                    {
                        modPlayer.planarSpeedBoost++;
                    }
                }
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            target.AddBuff(BuffID.Electrified, 180);
            if (projectile.owner == Main.myPlayer)
            {
                if (target.statLife <= 0)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<PlanarRipperExplosion>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                if (crit)
                {
                    if (modPlayer.planarSpeedBoost < 20)
                    {
                        modPlayer.planarSpeedBoost++;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 10;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.damage = (int)(projectile.damage * 0.6f);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, (Main.rand.NextBool(2) ? 93 : 92), 0.5f, 0f);

            for (int i = 0; i < 5; i++)
            {
                int dust = Dust.NewDust(projectile.Center, 1, 1, 132, projectile.velocity.X, projectile.velocity.Y, 0, default, 0.5f);
                Main.dust[dust].noGravity = true;
            }
            int num212 = Main.rand.Next(10, 20);
            for (int num213 = 0; num213 < num212; num213++)
            {
                int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, 135, 0f, 0f, 100, default, 2f);
                Main.dust[num214].velocity *= 2f;
                Main.dust[num214].noGravity = true;
            }
        }
    }
}
