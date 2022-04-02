using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BeamingBolt2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beaming Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 90 && target.CanBeChasedBy(projectile);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            if (projectile.ai[0] == 1f)
                tex = ModContent.GetTexture("CalamityMod/Projectiles/Magic/BeamingThorn");

            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;
            int randomDust = Utils.SelectRandom(Main.rand, new int[]
            {
                164,
                58,
                204
            });
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);

            if (projectile.timeLeft < 90)
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 600f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                int randomDust = Utils.SelectRandom(Main.rand, new int[]
                {
                    164,
                    58,
                    204
                });
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
