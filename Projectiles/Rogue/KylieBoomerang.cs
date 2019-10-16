using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class KylieBoomerang : ModProjectile
    {
        //This variable will be used for the stealth strike
        internal float timer = 0f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kylie");
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
            projectile.tileCollide = false;
            drawOffsetX = 6 * projectile.spriteDirection;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //Constant rotation
            projectile.rotation += 0.2f;

            projectile.ai[0]++;
            //Dust trail
            if (Main.rand.Next(15) == 0)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 7, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 100, default, 0f);
                Main.dust[d].position = projectile.Center;
            }
            //Constant sound effects
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 15;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }
            //Slopes REEEEEEEEEEEE
            if (projectile.ai[0] == 3f)
            {
                projectile.tileCollide = true;
            }
            //Decide the range of the boomerang depending on stealth
            if (projectile.ai[1] == 1)
            {
                timer = 20f;
            }
            else
            {
                timer = 40f;
            }
            //Home in on the player after certain time
            if (projectile.ai[0] >= timer)
            {
                projectile.tileCollide = false;
                Player target = Main.player[projectile.owner];
                float var2 = target.position.X + (target.width / 2);
                float var3 = target.position.Y + (target.height / 2);
                float homingstrenght = 11f;
                Vector2 vector1 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
                float var6 = var2 - vector1.X;
                float var7 = var3 - vector1.Y;
                float var8 = (float)Math.Sqrt(var6 * var6 + var7 * var7);
                var8 = homingstrenght / var8;
                var6 *= var8;
                var7 *= var8;
                projectile.velocity.X = (projectile.velocity.X * 20f + var6) / 21f;
                projectile.velocity.Y = (projectile.velocity.Y * 20f + var7) / 21f;
                //Kill projectile if it collides with player
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value2 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value2))
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //Start homing at player if you hit an enemy
            projectile.ai[0] = 90;
            projectile.tileCollide = false;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce off tiles and start homing on player if it hits a tile
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(0, projectile.position);
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 90;
            projectile.tileCollide = false;
            return false;

        }
    }
}
