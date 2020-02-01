using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IceRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Rain");
        }

        public override void SetDefaults()
        {
            projectile.aiStyle = 1;
            projectile.width = 12;
            projectile.height = 12;
            projectile.hostile = true;
            projectile.penetrate = -1;
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
			if (projectile.ai[0] == 0f)
				projectile.velocity.Y = projectile.velocity.Y + 0.2f;

            if (projectile.localAI[0] == 0f || projectile.localAI[0] == 2f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.localAI[0] = 1f;
                    projectile.alpha = 0;
                }
            }
            else if (projectile.localAI[0] == 1f)
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 50;
                if (projectile.alpha >= 255)
                {
                    projectile.localAI[0] = 2f;
                    projectile.alpha = 255;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
			Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 27, 0.25f);
			//Main.PlaySound(SoundID.Item27, projectile.position);
            for (int num373 = 0; num373 < 3; num373++)
            {
                int num374 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 76, 0f, 0f, 0, default, 1f);
                Main.dust[num374].noGravity = true;
                Main.dust[num374].noLight = true;
                Main.dust[num374].scale = 0.7f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60, true);
            target.AddBuff(BuffID.Chilled, 30, true);
        }
    }
}
