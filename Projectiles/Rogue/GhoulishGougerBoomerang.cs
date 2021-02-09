using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class GhoulishGougerBoomerang : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/GhoulishGouger";

        private const int FramesBeforeReturning = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghoulish Gouger");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 36;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Boomerang glows pink
            Lighting.AddLight(projectile.Center, 0.7f, 0f, 0.15f);

            // Frame 1, pick a direction for the scythe. This direction isn't changed from that point on
            if (projectile.ai[0] == 0f)
                projectile.spriteDirection = projectile.direction;

            // Main boomerang logic. projectile.ai[0] is a frame counter.
            projectile.ai[0] += 1f;

            // On the first returning frame, send a net update.
            if (projectile.ai[0] == FramesBeforeReturning)
                projectile.netUpdate = true;

            // Once returning, use boomerang return AI.
            if (projectile.ai[0] >= FramesBeforeReturning)
            {
                float returnSpeed = 16f;
                float acceleration = 1.15f;

                Player owner = Main.player[projectile.owner];
                Vector2 delta = owner.Center - projectile.Center;
                float dx = delta.X;
                float dy = delta.Y;

                // If the boomerang is excessively far away, destroy it.
                float dist = delta.Length();
                if (dist > 3000f)
                    projectile.Kill();

                // Homing vector math (the boomerang homes in on the player using rather ugly code)
                dist = returnSpeed / dist;
                dx *= dist;
                dy *= dist;

                // X/Y specific boomerang return code. This is what gives them their unique flight path.
                if (projectile.velocity.X < dx)
                {
                    projectile.velocity.X = projectile.velocity.X + acceleration;
                    if (projectile.velocity.X < 0f && dx > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + acceleration;
                    }
                }
                else if (projectile.velocity.X > dx)
                {
                    projectile.velocity.X = projectile.velocity.X - acceleration;
                    if (projectile.velocity.X > 0f && dx < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - acceleration;
                    }
                }
                if (projectile.velocity.Y < dy)
                {
                    projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    if (projectile.velocity.Y < 0f && dy > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + acceleration;
                    }
                }
                else if (projectile.velocity.Y > dy)
                {
                    projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    if (projectile.velocity.Y > 0f && dy < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - acceleration;
                    }
                }

                // Destroy the boomerang when it returns to the player.
                if (Main.myPlayer == projectile.owner)
                    if (projectile.Hitbox.Intersects(owner.Hitbox))
                        projectile.Kill();
            }

            // Rotate the scythe as it flies.
            float spin = projectile.spriteDirection <= 0 ? -1f : 1f;
            projectile.rotation += spin * 0.31f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(37f, 34f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/GhoulishGougerGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner != Main.myPlayer || !projectile.Calamity().stealthStrike)
                return;

            // Stealth strike on-hit souls can only happen once.
            // This prevents https://youtu.be/2vAWZhg1dBE
            if (projectile.numHits > 0)
                return;

            int numSouls = 8;
            int projID = ModContent.ProjectileType<PhantasmalSoul>();
            int soulDamage = (int)(projectile.damage * 0.75f);
            float soulKB = 0f;
            float speed = 6f;
            Vector2 velocity = Main.rand.NextVector2CircularEdge(speed, speed);
            for (int i = 0; i < numSouls; i += 2)
            {
                float ai1 = Main.rand.NextFloat() + 0.5f;
                Projectile.NewProjectile(projectile.Center, velocity, projID, soulDamage, soulKB, projectile.owner, 1f, ai1);
                Projectile.NewProjectile(projectile.Center, -velocity, projID, soulDamage, soulKB, projectile.owner, 1f, ai1);

                // Rotate direction for the next pair of souls.
                velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
            }
        }
    }
}
