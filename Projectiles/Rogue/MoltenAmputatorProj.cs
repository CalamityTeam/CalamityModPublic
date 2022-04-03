using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.timeLeft = 180;
            Projectile.Calamity().rogue = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
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
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, iAmSpeed.X, iAmSpeed.Y, ModContent.ProjectileType<MoltenBlobThrown>(), (int)(Projectile.damage * 0.25), 0f, Projectile.owner, 0f, 0f);
            }
        }

        // Manually implemented boomerang AI copied from Ghoulish Gouger
        // TODO -- this should be a utils function
        public override void AI()
        {
            // Stealth strikes constantly spawn molten blobs.
            if (Projectile.Calamity().stealthStrike)
            {
                // If the stealth blob timer isn't set up yet, set it up
                if (Projectile.ai[1] == 0f)
                    Projectile.ai[1] = FramesPerBlob;
                else
                {
                    Projectile.ai[1]--;
                    if (Projectile.ai[1] <= 0f)
                    {
                        SpawnBlobs(1);
                        Projectile.ai[1] = FramesPerBlob;
                    }
                }
            }

            // Frame 1, pick a direction for the scythe. This direction isn't changed from that point on
            if (Projectile.ai[0] == 0f)
                Projectile.spriteDirection = Projectile.direction;

            // Boomerang glows orange
            Lighting.AddLight(Projectile.Center, 0.65f, 0.45f, 0f);

            // Boomerang noises
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            // Main boomerang logic. projectile.ai[0] is a frame counter.
            Projectile.ai[0] += 1f;

            // On the first returning frame, send a net update.
            if (Projectile.ai[0] == FramesBeforeReturning)
                Projectile.netUpdate = true;

            // Once returning, use boomerang return AI.
            if (Projectile.ai[0] >= FramesBeforeReturning)
            {
                float returnSpeed = MoltenAmputator.Speed * 2f;
                float acceleration = 1.15f;

                Player owner = Main.player[Projectile.owner];
                Vector2 delta = owner.Center - Projectile.Center;
                float dx = delta.X;
                float dy = delta.Y;

                // If the boomerang is excessively far away, destroy it.
                float dist = delta.Length();
                if (dist > 3000f)
                    Projectile.Kill();

                // Homing vector math (the boomerang homes in on the player using rather ugly code)
                dist = returnSpeed / dist;
                dx *= dist;
                dy *= dist;

                // X/Y specific boomerang return code. This is what gives them their unique flight path.
                if (Projectile.velocity.X < dx)
                {
                    Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    if (Projectile.velocity.X < 0f && dx > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + acceleration;
                    }
                }
                else if (Projectile.velocity.X > dx)
                {
                    Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    if (Projectile.velocity.X > 0f && dx < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - acceleration;
                    }
                }
                if (Projectile.velocity.Y < dy)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    if (Projectile.velocity.Y < 0f && dy > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                    }
                }
                else if (Projectile.velocity.Y > dy)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    if (Projectile.velocity.Y > 0f && dy < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                    }
                }

                // Destroy the boomerang when it returns to the player.
                if (Main.myPlayer == Projectile.owner)
                    if (Projectile.Hitbox.Intersects(owner.Hitbox))
                        Projectile.Kill();
            }

            // Rotate the scythe as it flies.
            float spin = Projectile.spriteDirection <= 0 ? -1f : 1f;
            Projectile.rotation += spin * 0.38f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int blobCount = Projectile.Calamity().stealthStrike ? 4 : 2;
                SpawnBlobs(blobCount);
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int blobCount = Projectile.Calamity().stealthStrike ? 4 : 2;
                SpawnBlobs(blobCount);
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
