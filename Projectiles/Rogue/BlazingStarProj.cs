using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BlazingStarProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BlazingStar";

        public const int Lifetime = 1540;
        public const int ReboundTime = 40;

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
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = Lifetime;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.Calamity().stealthStrike)
                projectile.penetrate = projectile.maxPenetrate = -1;

            // Boomerang rotation
            projectile.rotation += 0.4f * projectile.direction;

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

                float returnSpeed = BlazingStar.Speed * 2.5f;
                float acceleration = 2f;
                Player owner = Main.player[projectile.owner];

                // Delete the projectile if it's excessively far away.
                // Relying on the length instead of a second square root call for Normalize is used here for general performance since both the distance
                // and normalized vector are used in this code.
                float distanceToPlayer = projectile.Distance(owner.Center);
                Vector2 velocity = (owner.Center - projectile.Center) / distanceToPlayer * returnSpeed;

                // Kill the projectile is if it's super far from the player.
                if (distanceToPlayer > 3000f)
                    projectile.Kill();

                // Home back in on the player.

                // No, projectiles do not have a SimpleFlyMovement extension method like NPCs do.
                if (projectile.velocity.X < velocity.X)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && velocity.X > 0f)
                        projectile.velocity.X += acceleration;
                }
                else if (projectile.velocity.X > velocity.X)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && velocity.X < 0f)
                        projectile.velocity.X -= acceleration;
                }
                if (projectile.velocity.Y < velocity.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && velocity.Y > 0f)
                        projectile.velocity.Y += acceleration;
                }
                else if (projectile.velocity.Y > velocity.Y)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && velocity.Y < 0f)
                        projectile.velocity.Y -= acceleration;
                }

                // Delete the projectile if it touches its owner.
                if (Main.myPlayer == projectile.owner)
                {
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
        // Make the star bounce on tiles.
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
