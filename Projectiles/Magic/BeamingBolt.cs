using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BeamingBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beaming Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.magic = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Magic/BeamingThornBlossom");

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            projectile.velocity.X *= 0.95f;
            projectile.velocity.Y *= 0.985f;
            for (int dust = 0; dust < 2; dust++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    164,
                    58,
                    204
                });
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 6; k++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    164,
                    58,
                    204
                });
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            float spread = 90f * 0.0174f;
            double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 4; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BeamingBolt2>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner, projectile.ai[0], 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<BeamingBolt2>(), (int)(projectile.damage * 0.75), projectile.knockBack, projectile.owner, projectile.ai[0], 0f);
                }
            }
            Main.PlaySound(SoundID.Item105, projectile.position);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 7;
        }
    }
}
