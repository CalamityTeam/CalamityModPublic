using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MoltenAmputatorProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/MoltenAmputator";
        private const int FramesBeforeReturning = 15;
        private const int FramesPerBlob = 8;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Amputator");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 74;
            projectile.height = 74;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 4;
            projectile.timeLeft = 180;
            projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        private void SpawnBlobs(int blobCount)
        {
            for (int i = 0; i < blobCount; i++)
            {
                Vector2 iAmSpeed = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                while (iAmSpeed.X == 0f && iAmSpeed.Y == 0f)
                {
                    iAmSpeed = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                }
                iAmSpeed.Normalize();
                iAmSpeed *= (float)Main.rand.Next(70, 101) * 0.1f;
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, iAmSpeed.X, iAmSpeed.Y, ModContent.ProjectileType<MoltenBlobThrown>(), (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
            }
        }

        // Manually implemented boomerang AI copied from Ghoulish Gouger
        // TODO -- this should be a utils function
        public override void AI()
        {
            // Stealth strikes constantly spawn molten blobs.
            if (projectile.Calamity().stealthStrike)
            {
                // If the stealth blob timer isn't set up yet, set it up
                if (projectile.ai[1] == 0f)
                    projectile.ai[1] = FramesPerBlob;
                else
                {
                    projectile.ai[1]--;
                    if (projectile.ai[1] <= 0f)
                    {
                        SpawnBlobs(1);
                        projectile.ai[1] = FramesPerBlob;
                    }
                }
            }

            // Frame 1, pick a direction for the scythe. This direction isn't changed from that point on
            if (projectile.ai[0] == 0f)
                projectile.spriteDirection = projectile.direction;

            // Boomerang glows orange
            Lighting.AddLight(projectile.Center, 0.65f, 0.45f, 0f);

            // Boomerang noises
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            // Main boomerang logic. projectile.ai[0] is a frame counter.
            projectile.ai[0] += 1f;

            // On the first returning frame, send a net update.
            if (projectile.ai[0] == FramesBeforeReturning)
                projectile.netUpdate = true;

            // Once returning, use boomerang return AI.
            if (projectile.ai[0] >= FramesBeforeReturning)
            {
                float returnSpeed = MoltenAmputator.Speed * 2f;
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
            projectile.rotation += spin * 0.38f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int blobCount = projectile.Calamity().stealthStrike ? 4 : 2;
                SpawnBlobs(blobCount);
            }
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                int blobCount = projectile.Calamity().stealthStrike ? 4 : 2;
                SpawnBlobs(blobCount);
            }
            Main.PlaySound(SoundID.Item20, projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 244, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
