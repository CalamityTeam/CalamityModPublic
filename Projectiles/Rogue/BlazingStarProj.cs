using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class BlazingStarProj : ModProjectile
    {
        private static int Lifetime = 1540;
        private static int ReboundTime = 75;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.penetrate = 5;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // ai[1] = 1 means that the projectile is a stealth strike, in which case it pierces infinitely.
            if (projectile.ai[1] == 1f)
                projectile.penetrate = projectile.maxPenetrate = -1;

            // Boomerang rotation
            projectile.rotation += 0.4f * (float)projectile.direction;

            // Boomerang sound
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Returns after some number of frames in the air
            if (projectile.timeLeft < Lifetime - ReboundTime)
                projectile.ai[0] = 1f;

            if (projectile.ai[0] != 0f)
            {
                projectile.tileCollide = false;

                float returnSpeed = BlazingStar.Speed;
                float acceleration = 0.35f;
                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
                Vector2 distance = owner.Center - projectile.Center;
                Vector2 ownerDistNorm = projectile.DirectionTo(owner.Center) * returnSpeed;
                if (distance.Length() > 3000f)
                    projectile.Kill();

                // Home back in on the player.
                if (projectile.velocity.X < distance.X)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && distance.X > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > distance.X)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && distance.X < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < distance.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && distance.Y > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > distance.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && distance.Y < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
        // Make it bounce on tiles.
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Impacts the terrain even though it bounces off.
            Main.PlaySound(SoundID.Dig, projectile.Center);
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);

            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            projectile.ai[0] = 1f;
            return false;
        }
    }
}
