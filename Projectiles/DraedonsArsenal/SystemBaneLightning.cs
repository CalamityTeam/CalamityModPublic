using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class SystemBaneLightning : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/LightningProj";

        public int ElectrocutionTarget
        {
            get => (int)projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        public const float OuterLightningScale = 0.5f;
        public const float InnerLightningScale = 0.3f;
        public const float OuterLightningOpacity = 0.35f;
        public const float InnerLightningOpacity = 0.75f;
        public static readonly Color OuterLightningColor = Color.Cyan;
        public static readonly Color InnerLightningColor = Color.White;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 60;
        }
        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.Calamity().rogue = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 40 * (projectile.extraUpdates + 1);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            // Initialize the lightning without a set target.
            if (projectile.localAI[0] == 0f)
            {
                ElectrocutionTarget = -1;
                projectile.localAI[0] = 1f;
            }
            bool hasSelectedTarget = ElectrocutionTarget >= 0 && ElectrocutionTarget < Main.npc.Length;
            NPC potentialTarget = projectile.Center.ClosestNPCAt(800f);
            if (hasSelectedTarget)
                potentialTarget = Main.npc[ElectrocutionTarget];
            if (potentialTarget != null)
            {
                // Don't move at all if the lightning is already close to/electrocuting its target.
                if (hasSelectedTarget && projectile.Distance(potentialTarget.Center) < 14f)
                    projectile.velocity = Vector2.Zero;
                else ArcToTarget(potentialTarget);
            }
            // Sometimes randomly update the velocity to make it look like it's actually lightning instead of just a line
            if (Main.rand.NextBool(10))
            {
                projectile.velocity = projectile.velocity.RotatedByRandom(0.52f);
                projectile.netUpdate = true;
            }
        }

        public void ArcToTarget(NPC target)
        {
            // Convert the velocity into a rotation, update it to move towards the target, and then convert that updated angle back into a velocity.
            float updatedVelocityDirection = projectile.velocity.ToRotation().AngleTowards(projectile.AngleTo(target.Center), 0.25f);
            projectile.velocity = updatedVelocityDirection.ToRotationVector2() * projectile.velocity.Length();

            // Sometimes add a bit of randomness to the lightning's movement.
            if (Main.rand.NextBool(5))
            {
                projectile.velocity = projectile.velocity.RotatedByRandom(0.9f);
                projectile.netSpam = 0;
                projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Immediately stop moving on touching an enemy to make it look like they're being electrocuted.
            if (ElectrocutionTarget == -1)
            {
                ElectrocutionTarget = target.whoAmI;
                projectile.velocity = Vector2.Zero;
                projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // If you have any ideas of making lightning that isn't the same as the cultist lightning and looks more like real lightning, please change this.
            List<Vector2> oldPositions = projectile.oldPos.Where(oldPosition => oldPosition != Vector2.Zero).ToList();
            for (int i = 0; i < oldPositions.Count - 1; i++)
            {
                // Draw two types of lightning to make it look like it has an outer and an inner part.

                // f_1 and c_1 are both used when determining the color of the lightning.
                // f_1 is a 0-1 color multiplier (used for opacity in this case),
                // and c_1 is the color to draw.
                DelegateMethods.f_1 = OuterLightningOpacity;
                DelegateMethods.c_1 = OuterLightningColor;

                Vector2 start = oldPositions[i] + projectile.Size * 0.5f - Main.screenPosition;
                Vector2 end = oldPositions[i + 1] + projectile.Size * 0.5f - Main.screenPosition;
                Utils.DrawLaser(spriteBatch, ModContent.GetTexture(Texture), start, end, new Vector2(OuterLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));

                DelegateMethods.f_1 = InnerLightningOpacity;
                DelegateMethods.c_1 = InnerLightningColor;
                Utils.DrawLaser(spriteBatch, ModContent.GetTexture(Texture), start, end, new Vector2(InnerLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
            return false;
        }
    }
}
