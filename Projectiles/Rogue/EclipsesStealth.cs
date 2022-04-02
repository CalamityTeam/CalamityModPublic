using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class EclipsesStealth : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/EclipsesFall";

        // For more consistent DPS, always alternates between spawning 1 and 2 spears instead of picking randomly
        private bool spawnTwoSpears = true;
        private bool changedTimeLeft = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eclipse's Fall");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 25;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        // Uses localAI[1] to decide how many frames until the next spear drops.
        public override void AI()
        {
            // Behavior when not sticking to anything
            if (projectile.ai[0] == 0f)
            {
                // Keep the spear oriented in the correct direction
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;

                // Spawn dust while flying
                if (Main.rand.NextBool(8))
                    Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 138, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }

            // Behavior when having impaled a target
            else
            {
                // Eclipse's Fall is guaranteed to impale for 10 seconds, no more, no less
                if (!changedTimeLeft)
                {
                    projectile.timeLeft = 600;
                    changedTimeLeft = true;
                }

                // Spawn spears. As this uses local AI, it's done client-side only.
                if (projectile.owner == Main.myPlayer)
                {
                    projectile.localAI[1] -= 1f;

                    if (projectile.localAI[1] <= 0f)
                    {
                        // Set up the spear counter for next time. Used to be every 5 frames there was a 50% chance; now it's more reliable but slower.
                        projectile.localAI[1] = Main.rand.Next(8, 14); // 8 to 13 frames between each spearfall

                        int type = ModContent.ProjectileType<EclipsesSmol>();
                        int smolDamage = (int)(projectile.damage * 0.22f);
                        float smolKB = 3f;
                        // Used to be a 50% chance each spearfall for 1 or 2. Now is consistent.
                        int numSpears = spawnTwoSpears ? 2 : 1;
                        spawnTwoSpears = !spawnTwoSpears;
                        for (int i = 0; i < numSpears; ++i)
                            CalamityUtils.ProjectileRain(projectile.Center, 400f, 100f, 500f, 800f, 29f, type, smolDamage, smolKB, projectile.owner);
                    }
                }
            }

            // Sticky behavior. Lets the projectile impale enemies and sticks to its impaled enemy automatically.
            projectile.StickyProjAI(10);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Impale the enemy on contact ("sticky behavior").
            projectile.ModifyHitNPCSticky(1, true);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
            {
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            }
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[0] == 1f)
            {
                return false;
            }
            return null;
        }

        public override bool CanHitPvp(Player target) => projectile.ai[0] != 1f;
    }
}
