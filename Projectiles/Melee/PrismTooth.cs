using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
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
    public class PrismTooth : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public const int Lifetime = 80;
        public Player Owner => Main.player[Projectile.owner];
        public ref float ShootReach => ref Projectile.ai[0];
        public ref float Time => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.usesIDStaticNPCImmunity = true;
            // THIS IS INTENTIONALLY VERY LOW. Do not increase this; it is essential for Photon Ripper to function properly.
            Projectile.idStaticNPCHitCooldown = 4;
            Projectile.timeLeft = Lifetime;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi * Time / Lifetime;

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
            positionOffset = positionOffset.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver2);

            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter) + Projectile.velocity * 42f + positionOffset;
            Projectile.Opacity = Utils.GetLerpValue(0f, 12f, Time, true) * Utils.GetLerpValue(Lifetime, Lifetime - 12f, Lifetime - Projectile.timeLeft, true);

            // Destroy trees within the range of the past 20 oldPos positions.
            for (int i = 0; i < 20; i++)
            {
                Point pointToCheck = (Projectile.oldPos[i] + Projectile.Size * 0.5f).ToTileCoordinates();
                AbsolutelyFuckingAnnihilateTrees(pointToCheck.X, pointToCheck.Y);
            }

            // Emit light.
            Lighting.AddLight(Projectile.Center, Vector3.One * 0.7f);

            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Projectile.RotatingHitboxCollision(targetHitbox.TopLeft(), targetHitbox.Size());

        public void AbsolutelyFuckingAnnihilateTrees(int x, int y)
        {
            Tile tileAtPosition = CalamityUtils.ParanoidTileRetrieval(x, y);

            // Ignore tiles that are not active and are not breakable by axes.
            if (!tileAtPosition.HasTile || !Main.tileAxe[tileAtPosition.TileType])
                return;

            // Don't attempt to mine the tile if for whatever reason it's not supposed to be broken.
            if (!WorldGen.CanKillTile(x, y))
                return;

            AchievementsHelper.CurrentlyMining = true;

            WorldGen.KillTile(x, y);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);

            AchievementsHelper.CurrentlyMining = false;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        internal float WidthFunction(float completionRatio) => Projectile.scale * 24f * (1f - Utils.GetLerpValue(0.7f, 1f, completionRatio, true)) + 1f;

        internal Color ColorFunction(float completionRatio)
        {
            float hue = (Projectile.identity % 9f / 9f + completionRatio * 0.7f) % 1f;
            return Color.Lerp(Color.White, Main.hslToRgb(hue, 0.95f, 0.55f), 0.35f) * Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Time <= 5f)
                return true;

            // Variable adjustment vector used to prevent the trail for starting somewhat that isn't behind
            // the crystal. This may appear in small amounts, with offsets of a few pixels, but at the speed
            // these crystals go, it's probably not something to worry too much about.
            Vector2 generalOffset = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 15f;
            generalOffset += Projectile.rotation.ToRotationVector2() * -5f * (float)Math.Sin(Projectile.rotation);

            // Mess with the oldPos array so that the trail always points towards the crystal.
            Vector2 oldPosition = Projectile.oldPos[1];
            Projectile.oldPos[1] = Projectile.oldPos[0] - Projectile.rotation.ToRotationVector2() * Vector2.Distance(Projectile.oldPos[0], Projectile.oldPos[1]);

            // Revert back if the above calculations caused any NaNs.
            if (Projectile.oldPos[1].HasNaNs())
                Projectile.oldPos[1] = oldPosition;

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:PrismaticStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            PrimitiveSet.Prepare(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f + generalOffset, shader: GameShaders.Misc["CalamityMod:PrismaticStreak"]), 65);
            Main.spriteBatch.ExitShaderRegion();
            return true;
        }

        // Prevent the crystals from utilizing velocity. Their movement is entirely dependant on Center setting.
        public override bool ShouldUpdatePosition() => false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
    }
}
