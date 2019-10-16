using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
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
        public override void AI()
        {
            if (projectile.localAI[0] == 0f)
            {
                projectile.position.Y -= projectile.height / 2; //position adjustments
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
