using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.minionSlots = 0.5f;
            projectile.timeLeft = 90000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.hide = true;
        }

        // Arbitrary function used to identify a projectile based owner and identity.
        // Used to get the correct projectile to attach to.
        // Do not EVER touch this.
        internal static bool SameIdentity(Projectile proj, int owner, int identity)
        {
            return proj.owner == owner && (proj.projUUID == identity || proj.identity == identity);
        }

        internal static void SegmentAI(Projectile projectile, int offsetFromNextSegment, ref int playerMinionSlots)
        {
            // If the mechworm is opaque enough, produce light.
            if (projectile.alpha <= 128)
                Lighting.AddLight(projectile.Center, Color.DarkMagenta.ToVector3());

            Player owner = Main.player[projectile.owner];
            CalamityPlayer modPlayer = owner.Calamity();

            // Track the minion presence boolean.
            if (owner.dead)
                modPlayer.mWorm = false;
            if (modPlayer.mWorm)
                projectile.timeLeft = 2;

            int headProjType = ModContent.ProjectileType<MechwormHead>();
            int bodyProjType = ModContent.ProjectileType<MechwormBody>();
            int tailProjType = ModContent.ProjectileType<MechwormTail>();

            ref float segmentAheadIdentity = ref projectile.ai[0];
            Projectile segmentAhead = Main.projectile.Take(Main.maxProjectiles).FirstOrDefault(proj => SameIdentity(proj, projectile.owner, (int)projectile.ai[0]));

            // Ensure that the segment ahead actually exists. If it doesn't, kill this segment.
            if (segmentAhead is null || !Main.projectile.IndexInRange(segmentAhead.whoAmI) || (segmentAhead.type != bodyProjType && segmentAhead.type != headProjType))
            {
                projectile.Kill();
                return;
            }

            // Delete segments if some are lost for whatever reason (such as a summon potion expiring).
            // playerMinionSlots is set to -1 for body segments to avoid type checking.
            if (playerMinionSlots != -1 && (owner.maxMinions < playerMinionSlots || !owner.active))
            {
                int lostSlots = playerMinionSlots - owner.maxMinions;
                while (lostSlots > 0)
                {
                    Projectile ahead = segmentAhead;
                    // Each body slot is actually 0.5 slots. Kill two segments to lose 1 "true" slot.
                    for (int i = 0; i < 2; ++i)
                    {
                        if (ahead.type != ModContent.ProjectileType<MechwormHead>())
                            projectile.localAI[1] = ahead.localAI[1];

                        // Inherit the ahead segment index of the ahead segment (basically attaching to the segment that's two indices ahead).
                        segmentAheadIdentity = ahead.ai[0];
                        projectile.netUpdate = true;

                        ahead.Kill();

                        // And re-decide the ahead segment.
                        segmentAhead = Main.projectile.Take(Main.maxProjectiles).FirstOrDefault(proj => SameIdentity(proj, projectile.owner, (int)projectile.ai[0]));

                        // Ensure that the segment ahead actually exists. If it doesn't, kill this segment.
                        if (segmentAhead is null || !Main.projectile.IndexInRange(segmentAhead.whoAmI))
                        {
                            projectile.Kill();
                            return;
                        }
                        ahead = segmentAhead;
                    }
                    lostSlots--;
                }
                playerMinionSlots = owner.maxMinions;
            }

            // Accumulate the total segments of the worm.
            segmentAhead.localAI[0] = projectile.localAI[0] + 1f;
            segmentAhead.Calamity().UpdatePriority = segmentAhead.localAI[0];

            // Locate the head segment by looping through the projectile array.
            // Doing identity checks across every segment would be much more expensive than doing just this one loop.
            Projectile head = LocateHead(projectile);

            // If no such head exists, kill this segment.
            if (head is null)
            {
                projectile.Kill();
                return;
            }

            // If the head is set to net update every body segment will also update.
            // This update cannot be blocked by netSpam.
            if (head.netUpdate)
            {
                projectile.netUpdate = true;
                if (projectile.netSpam > 59)
                    projectile.netSpam = 59;
            }

            MechwormHead headModProj = head.modProjectile as MechwormHead;
            if (headModProj.EndRiftGateUUID == -1)
            {
                // Very rapidly fade-in.
                projectile.alpha = Utils.Clamp(projectile.alpha - 16, 0, 255);
            }
            else if (projectile.Hitbox.Intersects(Main.projectile[headModProj.EndRiftGateUUID].Hitbox))
            {
                // Disappear if touching the mechworm portal.
                // It will look like it's teleporting, when in reality, it's
                // just an invisible, uninteractable projectile for the time being.
                projectile.alpha = 255;
            }

            projectile.velocity = Vector2.Zero;
            Vector2 offsetToDestination = segmentAhead.Center - projectile.Center;

            // This variant of segment attachment incorporates rotation.
            // Given the fact that all segments will execute this code is succession, the
            // result across the entire worm will exponentially decay over each segment,
            // allowing for smooth rotations. This code is what the stardust dragon uses for its segmenting.
            if (segmentAhead.rotation != projectile.rotation)
            {
                float offsetAngle = MathHelper.WrapAngle(segmentAhead.rotation - projectile.rotation);
                offsetToDestination = offsetToDestination.RotatedBy(offsetAngle * 0.08f);
            }
            projectile.rotation = offsetToDestination.ToRotation() + MathHelper.PiOver2;

            // Adjust the width/height of the segment in case the general size of the worm changes.
            if (offsetToDestination != Vector2.Zero)
                projectile.Center = segmentAhead.Center - offsetToDestination.SafeNormalize(Vector2.Zero) * offsetFromNextSegment;

            projectile.Center = Vector2.Clamp(projectile.Center, new Vector2(160f), new Vector2(Main.maxTilesX - 10, Main.maxTilesY - 10) * 16);
        }

        public static Projectile LocateHead(Projectile projectile)
        {
            int headType = ModContent.ProjectileType<MechwormHead>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type != headType || !Main.projectile[i].active || Main.projectile[i].owner != projectile.owner)
                    continue;
                return Main.projectile[i];
            }
            return null;
        }

        public override void AI()
        {
            int _ = -1;
            SegmentAI(projectile, 16, ref _);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Vector2 drawPosition = projectile.Center - Main.screenPosition;
            spriteBatch.Draw(tex, drawPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool CanDamage() => projectile.alpha == 0;

        public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
        {
            drawCacheProjsBehindProjectiles.Add(index);
        }

        public override bool ShouldUpdatePosition() => false;
    }
}
