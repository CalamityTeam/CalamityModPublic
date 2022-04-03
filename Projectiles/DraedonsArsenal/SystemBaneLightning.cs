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
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
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
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.Calamity().rogue = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40 * (Projectile.extraUpdates + 1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            // Initialize the lightning without a set target.
            if (Projectile.localAI[0] == 0f)
            {
                ElectrocutionTarget = -1;
                Projectile.localAI[0] = 1f;
            }
            bool hasSelectedTarget = ElectrocutionTarget >= 0 && ElectrocutionTarget < Main.npc.Length;
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(800f);
            if (hasSelectedTarget)
                potentialTarget = Main.npc[ElectrocutionTarget];
            if (potentialTarget != null)
            {
                // Don't move at all if the lightning is already close to/electrocuting its target.
                if (hasSelectedTarget && Projectile.Distance(potentialTarget.Center) < 14f)
                    Projectile.velocity = Vector2.Zero;
                else ArcToTarget(potentialTarget);
            }
            // Sometimes randomly update the velocity to make it look like it's actually lightning instead of just a line
            if (Main.rand.NextBool(10))
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(0.52f);
                Projectile.netUpdate = true;
            }
        }

        public void ArcToTarget(NPC target)
        {
            // Convert the velocity into a rotation, update it to move towards the target, and then convert that updated angle back into a velocity.
            float updatedVelocityDirection = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(target.Center), 0.25f);
            Projectile.velocity = updatedVelocityDirection.ToRotationVector2() * Projectile.velocity.Length();

            // Sometimes add a bit of randomness to the lightning's movement.
            if (Main.rand.NextBool(5))
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(0.9f);
                Projectile.netSpam = 0;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Immediately stop moving on touching an enemy to make it look like they're being electrocuted.
            if (ElectrocutionTarget == -1)
            {
                ElectrocutionTarget = target.whoAmI;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // If you have any ideas of making lightning that isn't the same as the cultist lightning and looks more like real lightning, please change this.
            List<Vector2> oldPositions = Projectile.oldPos.Where(oldPosition => oldPosition != Vector2.Zero).ToList();
            for (int i = 0; i < oldPositions.Count - 1; i++)
            {
                // Draw two types of lightning to make it look like it has an outer and an inner part.

                // f_1 and c_1 are both used when determining the color of the lightning.
                // f_1 is a 0-1 color multiplier (used for opacity in this case),
                // and c_1 is the color to draw.
                DelegateMethods.f_1 = OuterLightningOpacity;
                DelegateMethods.c_1 = OuterLightningColor;

                Vector2 start = oldPositions[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Vector2 end = oldPositions[i + 1] + Projectile.Size * 0.5f - Main.screenPosition;
                Utils.DrawLaser(spriteBatch, ModContent.Request<Texture2D>(Texture), start, end, new Vector2(OuterLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));

                DelegateMethods.f_1 = InnerLightningOpacity;
                DelegateMethods.c_1 = InnerLightningColor;
                Utils.DrawLaser(spriteBatch, ModContent.Request<Texture2D>(Texture), start, end, new Vector2(InnerLightningScale), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
            return false;
        }
    }
}
