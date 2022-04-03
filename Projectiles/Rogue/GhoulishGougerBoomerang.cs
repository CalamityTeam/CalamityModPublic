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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 36;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            // Boomerang glows pink
            Lighting.AddLight(Projectile.Center, 0.7f, 0f, 0.15f);

            // Frame 1, pick a direction for the scythe. This direction isn't changed from that point on
            if (Projectile.ai[0] == 0f)
                Projectile.spriteDirection = Projectile.direction;

            // Main boomerang logic. projectile.ai[0] is a frame counter.
            Projectile.ai[0] += 1f;

            // On the first returning frame, send a net update.
            if (Projectile.ai[0] == FramesBeforeReturning)
                Projectile.netUpdate = true;

            // Once returning, use boomerang return AI.
            if (Projectile.ai[0] >= FramesBeforeReturning)
            {
                float returnSpeed = 16f;
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
            Projectile.rotation += spin * 0.31f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 origin = new Vector2(37f, 34f);
            spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Rogue/GhoulishGougerGlow"), Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner != Main.myPlayer || !Projectile.Calamity().stealthStrike)
                return;

            // Stealth strike on-hit souls can only happen once.
            // This prevents https://youtu.be/2vAWZhg1dBE
            if (Projectile.numHits > 0)
                return;

            int numSouls = 8;
            int projID = ModContent.ProjectileType<PhantasmalSoul>();
            int soulDamage = Projectile.damage;
            float soulKB = 0f;
            float speed = 6f;
            Vector2 velocity = Main.rand.NextVector2CircularEdge(speed, speed);
            for (int i = 0; i < numSouls; ++i)
            {
                // Each soul has randomized player homing strength
                float ai1 = Main.rand.NextFloat() + 0.5f;
                Projectile.NewProjectile(Projectile.Center, velocity, projID, soulDamage, soulKB, Projectile.owner, 0f, ai1);

                // Rotate direction for the next soul
                velocity = velocity.RotatedBy(MathHelper.TwoPi / numSouls);
            }
        }
    }
}
