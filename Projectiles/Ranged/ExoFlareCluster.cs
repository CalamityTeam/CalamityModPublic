﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    // Photoviscerator right click main projectile (invisible flare cluster bomb)
    public class ExoFlareCluster : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float MinDistanceFromTarget = 45f;
        public const float MaxDistanceFromTarget = 350f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Flare Cluster");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 50;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            // localAI[0] is used by the sticky AI method, so use localAI[1] to spawn the flares.
            if (projectile.localAI[1] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int projID = ModContent.ProjectileType<ExoFlare>();
                    int flareDamage = (int)(0.6f * projectile.damage);
                    float flareKB = projectile.knockBack;
                    for (int i = 0; i < 4; i++)
                    {
                        Projectile p = Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, projID, flareDamage, flareKB, projectile.owner);
                        p.localAI[1] = Projectile.GetByUUID(projectile.owner, projectile.whoAmI);
                    }
                }
                projectile.localAI[1] = 1f;
            }

            if (projectile.ai[0] == 0f)
            {
                NPC potentialTarget = projectile.Center.ClosestNPCAt(MaxDistanceFromTarget, true, true);
                if (potentialTarget != null)
                {
                    if (projectile.Distance(potentialTarget.Center) > MinDistanceFromTarget)
                    {
                        float angleOffset = projectile.AngleTo(potentialTarget.Center) - projectile.velocity.ToRotation();
                        angleOffset = MathHelper.WrapAngle(angleOffset);
                        projectile.velocity = projectile.velocity.RotatedBy(MathHelper.Clamp(angleOffset, -0.1f, 0.1f));
                    }
                }
            }
            projectile.StickyProjAI(5);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            projectile.ModifyHitNPCSticky(4, true);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.width == 50)
            {
                int width = (int)MathHelper.Min(target.Hitbox.Width, 60);
                int height = (int)MathHelper.Min(target.Hitbox.Height, 60);
                CalamityGlobalProjectile.ExpandHitboxBy(projectile, width, height);
            }
            target.ExoDebuffs(2f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs(2f);
        }
    }
}