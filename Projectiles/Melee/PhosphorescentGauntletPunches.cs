using CalamityMod.DataStructures;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PhosphorescentGauntletPunches : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public bool HasPerformedLunge
        {
            get => projectile.ai[0] == 1f;
            set
            {
                int newValue = value.ToInt();
                if (projectile.ai[0] != newValue)
                {
                    projectile.ai[0] = newValue;
                    projectile.netUpdate = true;
                }
            }
        }

        public ref float Time => ref projectile.ai[1];

        public const float LungeSpeed = 19f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Punch");
            Main.projFrames[projectile.type] = 14;
        }

        public override void SetDefaults()
        {
            projectile.scale = 1.6f;
            projectile.width = projectile.height = (int)(projectile.scale * 60);
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ownerHitCheck = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
            projectile.frameCounter = 0;
            projectile.Calamity().trueMelee = true;
        }

        #region AI
        public override void AI()
        {
            if (!HasPerformedLunge)
                PerformLunge();

            Vector2 topLeft = projectile.Center + projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 40f;
            Vector2 topRight = projectile.Center + projectile.velocity.RotatedBy(MathHelper.PiOver2) * 40f;
            if (Time >= 8f && !Collision.CanHitLine(topLeft, 8, 8, topRight, 8, 8))
                ReelBack();
            HandleProjectileVisuals();
            HandlePositioning();
            Time++;
        }

        internal void PerformLunge()
        {
            if (Main.myPlayer != projectile.owner)
                return;
            Owner.velocity = projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction) * LungeSpeed;
            HasPerformedLunge = true;
        }

        internal void ReelBack()
        {
            Owner.GiveIFrames(PhosphorescentGauntlet.OnHitIFrames);

            // Create some visual effects.
            if (!Main.dedServ)
            {
                Vector2 topLeft = projectile.Center + projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 40f;
                Vector2 top = projectile.Center + projectile.velocity * 70f;
                Vector2 topRight = projectile.Center + projectile.velocity.RotatedBy(MathHelper.PiOver2) * 40f;
                foreach (Vector2 spawnPosition in new BezierCurve(topLeft, top, topRight).GetPoints(50))
                {
                    Dust sulphurousAcid = Dust.NewDustPerfect(spawnPosition + projectile.velocity * 16f, (int)CalamityDusts.SulfurousSeaAcid);
                    sulphurousAcid.velocity = projectile.velocity * 4f;
                    sulphurousAcid.noGravity = true;
                    sulphurousAcid.scale = 1.2f;
                }
            }
            if (Main.myPlayer != projectile.owner)
                return;

            // Reel back after collision.
            Owner.velocity = Vector2.Reflect(Owner.velocity.SafeNormalize(Vector2.Zero), projectile.velocity.SafeNormalize(Vector2.Zero)) * Owner.velocity.Length();

            // Create on-hit tile dust.
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width + 16, projectile.height + 16);
            projectile.Kill();
        }

        internal static void GenerateDustOnOwnerHand(Player player)
        {
            if (Main.dedServ)
                return;

            Vector2 handOffset = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
                handOffset.X = player.bodyFrame.Width - handOffset.X;
            if (player.gravDir != 1f)
                handOffset.Y = player.bodyFrame.Height - handOffset.Y;

            handOffset -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - player.height) / 2f;
            Vector2 rotatedHandPosition = player.RotatedRelativePoint(player.position + handOffset, true);
            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 150, default, 1.3f);
                dust.position = rotatedHandPosition;
                dust.velocity = Vector2.Zero;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity += player.velocity;
                if (Main.rand.NextBool(2))
                {
                    dust.position += Utils.RandomVector2(Main.rand, -4f, 4f);
                    dust.scale += Main.rand.NextFloat();
                }
            }
        }

        internal void HandleProjectileVisuals()
        {
            float velocityAngle = projectile.velocity.ToRotation();
            projectile.rotation = velocityAngle + MathHelper.Pi;
            projectile.frameCounter++;
            if (projectile.frameCounter % 3 == 2)
            {
                projectile.frame++;

                // Die at the end of the final punch.
                if (projectile.frame >= Main.projFrames[projectile.type])
                    projectile.Kill();
            }
        }

        internal void HandlePositioning()
        {
            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter);
            projectile.Center += projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction) * 30f;
        }
        #endregion

        #region Drawing

        // Manual drawing is used to correct the origin of the projectile when drawn.
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D punchTexture = ModContent.GetTexture(Texture);
            Rectangle frame = punchTexture.Frame(1, Main.projFrames[projectile.type], 0, projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            SpriteEffects directionEffect = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(punchTexture, projectile.Center - Main.screenPosition, frame, lightColor, projectile.rotation, origin, projectile.scale, directionEffect, 0f);
            return false;
        }
        #endregion

        #region NPC Hit Collision Logic

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => ReelBack();
        #endregion
    }
}
