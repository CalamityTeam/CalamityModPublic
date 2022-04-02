using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Bubble");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.timeLeft = 480;
            projectile.ranged = true;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
            if (projectile.timeLeft < 475)
            {
                for (int num105 = 0; num105 < 2; num105++)
                {
                    float num99 = projectile.velocity.X / 3f * (float)num105;
                    float num100 = projectile.velocity.Y / 3f * (float)num105;
                    int num101 = 4;
                    int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 1.2f);
                    Dust dust = Main.dust[num102];
                    dust.noGravity = true;
                    dust.velocity *= 0.1f;
                    dust.velocity += projectile.velocity * 0.1f;
                    dust.position.X -= num99;
                    dust.position.Y -= num100;
                }
                if (Main.rand.NextBool(10))
                {
                    int num103 = 4;
                    int num104 = Dust.NewDust(new Vector2(projectile.position.X + (float)num103, projectile.position.Y + (float)num103), projectile.width - num103 * 2, projectile.height - num103 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 0.6f);
                    Main.dust[num104].velocity *= 0.25f;
                    Main.dust[num104].velocity += projectile.velocity * 0.5f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(60, Main.DiscoG, 190, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item96, projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 202, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, new Color(60, Main.DiscoG, 190));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            Main.PlaySound(SoundID.Item96, projectile.Center);
            if (projectile.ai[0] == 1f)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (projectile.owner == Main.myPlayer)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Projectile bubble = CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(20f, 25f), ModContent.ProjectileType<SeasSearingBubble>(), projectile.damage / 2, 1f, projectile.owner);
                        bubble.rotation = angle;
                        bubble.tileCollide = false;
                        bubble.usesLocalNPCImmunity = true;
                        bubble.localNPCHitCooldown = -1;
                    }
                }
            }
        }
    }
}
