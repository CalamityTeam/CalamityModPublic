using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ExoLightBurst : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public const float MinDistanceFromTarget = 45f;
        public const float MaxDistanceFromTarget = 1350f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Flare");
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 190;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.timeLeft = 180;
        }
        public override void AI()
        {
            // localAI[0] is used by the sticky AI method.
            if (projectile.localAI[1] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Projectile.NewProjectileDirect(projectile.Center,
                                                       Vector2.Zero,
                                                       ModContent.ProjectileType<ExoLight>(),
                                                       projectile.damage,
                                                       projectile.knockBack,
                                                       projectile.owner).localAI[1] = Projectile.GetByUUID(projectile.owner, projectile.whoAmI);
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
            projectile.ModifyHitNPCSticky(4, false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int width = (int)MathHelper.Min(targetHitbox.Width, 150);
            int height = (int)MathHelper.Min(targetHitbox.Height, 150);
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, width, height);
            return null;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.ExoDebuffs(2f);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
			target.ExoDebuffs(2f);
        }
    }
}
