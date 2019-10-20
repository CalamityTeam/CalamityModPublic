using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Projectiles.Melee
{
    public class NadirSpear : ModProjectile
    {
        private const float StartOffset = 5.5f;
        private const float OutSpeed = 1.1f;
        private const float BackSpeed = 2.1f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nadir");
        }

        public override void SetDefaults()
        {
            projectile.width = 55;
            projectile.height = 55;
            projectile.aiStyle = 19;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;

            projectile.hide = true;

            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
        }

        // ai[0] is the current offset of the spear. Positive is further away from the player.
        public override void AI()
        {
            // On the first frame, set the spear at its starting offset
            if (projectile.ai[0] == 0f)
            {
                projectile.ai[0] = StartOffset;
                projectile.netUpdate = true;
            }

            // Update the player so that he/she is still holding the spear
            Player player = Main.player[projectile.owner];
            player.direction = projectile.direction;
            player.heldProj = projectile.whoAmI;
            player.itemTime = player.itemAnimation;

            // Update the spear so that it continues to be "held" by the player every frame
            projectile.position.X = player.position.X + (player.width / 2) - (projectile.width / 2);
            projectile.position.Y = player.position.Y + (player.height / 2) - (projectile.height / 2);

            // Move the spear based on its current offset
            projectile.position += projectile.velocity * projectile.ai[0];

            // The spear is returning for the last 1/3 of the item's animation
            if (player.itemAnimation < player.itemAnimationMax / 3f)
            {
                projectile.ai[0] -= BackSpeed;

                // On the very first returning frame, fire the projectile
                if (projectile.localAI[0] == 0f)
                {
                    projectile.localAI[0] = 1f;

                    int damage = Nadir.BaseDamage / 5;
                    float kb = 5.5f;
                    Vector2 projPos = projectile.Center + projectile.velocity;
                    Vector2 projVel = projectile.velocity * 0.75f;
                    if (projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(projPos, projVel, ModContent.ProjectileType<VoidEssence>(), damage, kb, projectile.owner, 0f, 0f);

                    // Play a screaming soul sound effect (unused Shadowflame Hex Doll noise)
                    Main.PlaySound(SoundID.Item104, projectile.Center);

                    // Create a circle of purple dust where the projectile comes out, looking like the edge of a portal
                    int circleDust = 18;
                    Vector2 baseDustVel = new Vector2(3.8f, 0f);
                    for (int i = 0; i < circleDust; ++i)
                    {
                        int dustID = 27;
                        float angle = i * (MathHelper.TwoPi / circleDust);
                        Vector2 dustVel = baseDustVel.RotatedBy(angle);

                        int idx = Dust.NewDust(projectile.Center, 1, 1, dustID);
                        Main.dust[idx].noGravity = true;
                        Main.dust[idx].position = projectile.Center;
                        Main.dust[idx].velocity = dustVel;
                        Main.dust[idx].scale = 2.4f;
                    }
                }
            }

            // For the remaining 2/3, the spear is heading out and doing nothing.
            else
                projectile.ai[0] += OutSpeed;

            // If the player is no longer using an item (e.g. they die), kill the spear instantly.
            if (Main.player[projectile.owner].itemAnimation == 0)
                projectile.Kill();

            // Produce dust at all times while the spear is moving.
            int movingDust = 3;
            for (int i = 0; i < movingDust; ++i)
            {
                int dustID = Main.rand.NextBool(4) ? 27 : 118;
                Vector2 corner = 0.5f * projectile.position + 0.5f * projectile.Center;
                int idx = Dust.NewDust(corner, projectile.width / 2, projectile.height / 2, dustID);

                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Vector2.Zero;
            }
        }
    }
}
