using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
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
            projectile.localNPCHitCooldown = 5;
            projectile.hide = true;
        }
        public static int LocalIndexFromIdentity(int owner, int uuid)
        {
            int indexFromUUID = Projectile.GetByUUID(owner, uuid);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].identity != indexFromUUID || !Main.projectile[i].active || Main.projectile[i].owner != owner)
                    continue;
                return i;
            }
            return -1;
        }

        public static void SegmentAI(Projectile projectile, int offsetFromNextSegment, ref int playerMinionSlots)
        {
            if (projectile.alpha <= 128)
                Lighting.AddLight(projectile.Center, Color.DarkMagenta.ToVector3());
            Player owner = Main.player[projectile.owner];
            CalamityPlayer modPlayer = owner.Calamity();

            // Minion stuff.
            if (owner.dead)
                modPlayer.mWorm = false;

            if (modPlayer.mWorm)
                projectile.timeLeft = 2;

            // Sync the entire worm every 2 seconds
            // This has potential to cause a packet storm, but only if the player is cheating in some way by freezing time.
            if ((int)Main.time % 120 == 0)
                projectile.netUpdate = true;

            ref float aheadSegmentIndex = ref projectile.ai[0];
            int aheadSegmentUUID = LocalIndexFromIdentity(projectile.owner, (int)aheadSegmentIndex);
            
            // Ensure that the segment ahead actually exists. If it doesn't, kill this segment.
            if (!Main.projectile.IndexInRange(aheadSegmentUUID))
            {
                projectile.Kill();
                return;
            }

            // Delete segments if some are lost for whatever reason (such as a summon potion 
            if (projectile.type == ModContent.ProjectileType<MechwormTail>() && (!owner.active || owner.maxMinions < playerMinionSlots))
            {
                int lostSlots = playerMinionSlots - owner.maxMinions;
                while (lostSlots > 0)
                {
                    Projectile ahead = Main.projectile[aheadSegmentUUID];
                    // Each body slot is actually 0.5 slots. Kill two segments to lose 1 "true" slot.
                    for (int i = 0; i < 2; i++)
                    {
                        if (ahead.type != ModContent.ProjectileType<MechwormHead>())
                            projectile.localAI[1] = ahead.localAI[1];

                        // Inherit the ahead segment index of the ahead segment (basically attaching to the segment that's two indices ahead).
                        aheadSegmentIndex = ahead.ai[0];
                        projectile.netUpdate = true;

                        ahead.Kill();

                        // And re-decide the ahead segment UUID.
                        aheadSegmentUUID = LocalIndexFromIdentity(projectile.owner, (int)aheadSegmentIndex);

                        // Ensure that the segment ahead actually exists. If it doesn't, kill this segment.
                        if (!Main.projectile.IndexInRange(aheadSegmentUUID))
                        {
                            projectile.Kill();
                            return;
                        }
                        ahead = Main.projectile[aheadSegmentUUID];
                    }
                    lostSlots--;
                }
                playerMinionSlots = owner.maxMinions;
            }

            Projectile segmentAhead = Main.projectile[aheadSegmentUUID];
            Projectile head = segmentAhead;

            // Accumulate the total segments of the worm.
            segmentAhead.localAI[0] = projectile.localAI[0] + 1f;

            // Delete the player's mechworm if it's attaching to something weird
            if (segmentAhead.type != ModContent.ProjectileType<MechwormBody>() &&
                segmentAhead.type != ModContent.ProjectileType<MechwormHead>())
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    bool isProjectileMechwormSegment =
                        proj.type == ModContent.ProjectileType<MechwormHead>() ||
                        proj.type == ModContent.ProjectileType<MechwormBody>() ||
                        proj.type == ModContent.ProjectileType<MechwormTail>();

                    if (proj.active && proj.owner == projectile.owner && isProjectileMechwormSegment)
                        proj.Kill();
                }
                return;
            }

            // Locate the head segment by looping through all worm segments until it is found.

            int tries = 0;
            while (head.type != ModContent.ProjectileType<MechwormHead>())
            {
                int aheadUUID = LocalIndexFromIdentity(head.owner, (int)head.ai[0]);
                if (aheadUUID == -1)
                {
                    projectile.Kill();
                    return;
                }

                head = Main.projectile[aheadUUID];
                
                if (tries >= Main.maxProjectiles)
                {
                    projectile.Kill();
                    return;
                }
                tries++;
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

        public override void PostAI()
        {
            int _ = 0;
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
