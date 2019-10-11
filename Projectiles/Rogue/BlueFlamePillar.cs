using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlueFlamePillar : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Pillar");
		}

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 316;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.Calamity().rogue = true;
		}
        /// <summary>
        /// Checks if a tile is below a designated Vector2
        /// </summary>
        /// <param name="position">The position to check</param>
        /// <param name="usingTileCoords">If the position provided is in tile coordinates or not</param>
        /// <returns></returns>
        public static bool SolidTileBelow(Vector2 position, bool usingTileCoords = false)
        {
            //if not in tile coordinates
            if (!usingTileCoords)
            {
                //convert them accordingly
                position = position.ToTileCoordinates().ToVector2();
            }
            int x = (int)position.X;
            int y = (int)position.Y;
            return Main.tile[x,y + 1].active() && Main.tileSolid[Main.tile[x, y + 1].type];
        }
        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.position.Y -= projectile.height / 2; //position adjustments
                if (!SolidTileBelow(projectile.Bottom))
                {
                    projectile.Kill();
                }
                projectile.localAI[0] = 1f;
            }
            float max = (float)(projectile.width * projectile.height) / 222f;
            int counter = 0;
            while ((float)counter < max)
            {
                int dustIndex = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0f, 0f, 100, default, 1f);
                Main.dust[dustIndex].noGravity = true;
                Dust dustFromIndex = Main.dust[dustIndex];
                dustFromIndex.velocity *= 0.5f;
                Dust dustFromIndex2 = Main.dust[dustIndex];
                dustFromIndex2.velocity.Y -= 0.5f;
                Main.dust[dustIndex].scale = 1.4f;
                Dust dustFromIndex3 = Main.dust[dustIndex];
                dustFromIndex3.position.X += 6f;
                Dust dustFromIndex4 = Main.dust[dustIndex];
                dustFromIndex4.position.Y -= 2f;
                counter++;
            }
        }
    }
}
