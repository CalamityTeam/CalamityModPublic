using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class SporeGasPlantera : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore Gas");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
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
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 1800f)
			{
                projectile.localAI[0] += 10f;
				projectile.damage = 0;
			}

            if (projectile.localAI[0] > 255f)
            {
                projectile.Kill();
                projectile.localAI[0] = 255f;
            }

            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.16f / 255f, (255 - projectile.alpha) * 0.2f / 255f, (255 - projectile.alpha) * 0.04f / 255f);

            projectile.alpha = (int)(100.0 + projectile.localAI[0] * 0.7);
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.rotation += projectile.direction * 0.002f;

            if (projectile.velocity.Length() > 0.5f)
                projectile.velocity *= 0.98f;
        }

        public override bool CanHitPlayer(Player target)
		{
            if (projectile.ai[1] > 1800f)
            {
                return false;
            }
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Changes the texture of the projectile
            Texture2D texture = Main.projectileTexture[projectile.type];
            switch ((int)projectile.ai[0])
            {
                case 0:
                    break;
                case 1:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Boss/SporeGasPlantera2");
                    break;
                case 2:
                    texture = ModContent.GetTexture("CalamityMod/Projectiles/Boss/SporeGasPlantera3");
                    break;
                default:
                    break;
            }
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1, texture);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 240);
            target.AddBuff(BuffID.Venom, 120);
        }
    }
}
