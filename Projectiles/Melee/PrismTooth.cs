using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class PrismTooth : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;
        public const int Lifetime = 80;
        public Player Owner => Main.player[projectile.owner];
        public ref float ShootReach => ref projectile.ai[0];
        public ref float Time => ref projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photon Ripper");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 52;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 5;
            projectile.usesIDStaticNPCImmunity = true;
            // THIS IS INTENTIONALLY VERY LOW. Do not increase this; it is essential for Photon Ripper to function properly.
            projectile.idStaticNPCHitCooldown = 4;
            projectile.timeLeft = Lifetime;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi * Time / Lifetime;

            Vector2 baseDirection = (MathHelper.TwoPi * Time / Lifetime - MathHelper.PiOver2).ToRotationVector2();
            baseDirection.X *= 0.25f;

            // Constrain the Y offset into the bounds of 0-1 instead of -1-1. This prevents
            // the crystal from flying behind the owner. In this context, the Y offset becomes how far away the
            // crystal is from its own in terms of reach.
            baseDirection.Y = baseDirection.Y * 0.5f + 0.5f;
            Vector2 positionOffset = baseDirection * ShootReach;

            // Don't allow the X offset to go too far.
            // This hard limit turns the squashed circle into bending, semi-rectangular shape.
            if (Math.Abs(positionOffset.X) > 45f)
                positionOffset.X = Math.Sign(baseDirection.X) * 45f;

            // In this context, the velocity is simply the initial direction as a unit vector- it does not
            // actually influence movement in any way.
            positionOffset = positionOffset.RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver2);

            projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter) + projectile.velocity * 42f + positionOffset;
            projectile.Opacity = Utils.InverseLerp(0f, 12f, Time, true) * Utils.InverseLerp(Lifetime, Lifetime - 12f, Lifetime - projectile.timeLeft, true);

            // Destroy trees within the range of the past 20 oldPos positions.
            for (int i = 0; i < 20; i++)
            {
                Point pointToCheck = (projectile.oldPos[i] + projectile.Size * 0.5f).ToTileCoordinates();
                AbsolutelyFuckingAnnihilateTrees(pointToCheck.X, pointToCheck.Y);
            }

            // Emit light.
            Lighting.AddLight(projectile.Center, Vector3.One * 0.7f);

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());

        public void AbsolutelyFuckingAnnihilateTrees(int x, int y)
        {
            Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(x, y);

            // Ignore tiles that are not active and are not breakable by axes.
            if (!tileAtPosition.active() || !Main.tileAxe[tileAtPosition.type])
                return;

            // Don't attempt to mine the tile if for whatever reason it's not supposed to be broken.
            if (!WorldGen.CanKillTile(x, y))
                return;

            AchievementsHelper.CurrentlyMining = true;

            WorldGen.KillTile(x, y);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.TileChange, -1, -1, null, 0, x, y);

            AchievementsHelper.CurrentlyMining = false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        internal float WidthFunction(float completionRatio) => projectile.scale * 24f * (1f - Utils.InverseLerp(0.7f, 1f, completionRatio, true)) + 1f;

        internal Color ColorFunction(float completionRatio)
        {
            float hue = (projectile.identity % 9f / 9f + completionRatio * 0.7f) % 1f;
            return Color.Lerp(Color.White, Main.hslToRgb(hue, 0.95f, 0.55f), 0.35f) * projectile.Opacity;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Time <= 5f)
                return true;

            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:PrismaticStreak"]);

            // Variable adjustment vector used to prevent the trail for starting somewhat that isn't behind
            // the crystal. This may appear in small amounts, with offsets of a few pixels, but at the speed
            // these crystals go, it's probably not something to worry too much about.
            Vector2 generalOffset = projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 15f;
            generalOffset += projectile.rotation.ToRotationVector2() * -5f * (float)Math.Sin(projectile.rotation);

            // Mess with the oldPos array so that the trail always points towards the crystal.
            Vector2 oldPosition = projectile.oldPos[1];
            projectile.oldPos[1] = projectile.oldPos[0] - projectile.rotation.ToRotationVector2() * Vector2.Distance(projectile.oldPos[0], projectile.oldPos[1]);

            // Revert back if the above calculations caused any NaNs.
            if (projectile.oldPos[1].HasNaNs())
                projectile.oldPos[1] = oldPosition;

            spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:PrismaticStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));

            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f + generalOffset - Main.screenPosition, 65);
            spriteBatch.ExitShaderRegion();
            return true;
        }

        // Prevent the crystals from utilizing velocity. Their movement is entirely dependant on Center setting.
        public override bool ShouldUpdatePosition() => false;
    }
}
