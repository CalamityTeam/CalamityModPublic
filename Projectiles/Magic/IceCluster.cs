using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class IceCluster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 90;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.coldDamage = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f;

            if (projectile.localAI[1] == 0f)
            {
                projectile.localAI[1] = 1f;
                Main.PlaySound(SoundID.Item120, projectile.position);
            }

            projectile.ai[0] += 1f;
            if (projectile.ai[1] == 1f)
            {
                if (projectile.ai[0] % 30f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 vector80 = projectile.rotation.ToRotationVector2();
                    Projectile.NewProjectile(projectile.Center, vector80, ModContent.ProjectileType<IceCluster>(), projectile.damage, projectile.knockBack, projectile.owner);
                }

                Lighting.AddLight(projectile.Center, 0.3f, 0.75f, 0.9f);
            }

            if (projectile.ai[0] >= 90f)
                projectile.alpha += 25;
            else
                projectile.alpha -= 15;

            if (projectile.alpha < 0)
                projectile.alpha = 0;
            if (projectile.alpha > 255)
                projectile.alpha = 255;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;

            target.AddBuff(ModContent.BuffType<GlacialState>(), 30);

            Vector2 vector80 = projectile.rotation.ToRotationVector2();
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, vector80, ModContent.ProjectileType<IceCluster>(), projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}
