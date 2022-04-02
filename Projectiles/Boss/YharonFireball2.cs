using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class YharonFireball2 : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/YharonFireball";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Fireball");
            Main.projFrames[projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 34;
            projectile.height = 34;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 3600;
            cooldownSlot = 1;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
                projectile.frame = 0;

            if (projectile.velocity.Y < -1f)
            {
                // 129 frames to get from -50 to -1
                projectile.velocity.Y *= 0.97f;
            }
            else
            {
                // 85 frames to get from -1 to 16
                projectile.velocity.Y += 0.2f;
                if (projectile.velocity.Y > 16f)
                    projectile.velocity.Y = 16f;
            }

            projectile.velocity.X *= 0.995f;

            projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[0] = 1f;
                Main.PlayTrackedSound(SoundID.DD2_BetsyFireballShot, projectile.Center);
            }

            if (projectile.ai[0] >= 2f)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }

            if (Main.rand.NextBool(16))
            {
                Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 200, default, 1f);
                dust.scale *= 0.7f;
                dust.velocity += projectile.velocity * 0.25f;
            }
        }

        public override bool CanHitPlayer(Player target) => projectile.velocity.Y >= -16f;

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, projectile.alpha);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 14, 0.5f, 0f);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 144);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 100, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, 55, 0f, 0f, 100, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
            projectile.Damage();
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (projectile.velocity.Y >= -16f)
                target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 180);
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)    
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
