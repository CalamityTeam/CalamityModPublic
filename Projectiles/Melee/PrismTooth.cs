using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
	public class PrismTooth : ModProjectile
	{
		internal PrimitiveTrail TrailDrawer;
		public const int Lifetime = 30;
		public Player Owner => Main.player[projectile.owner];
		public ref float Time => ref projectile.ai[0];
		public ref float ShootReach => ref projectile.ai[1];
		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Tooth");
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 36;
		}

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 35;
			projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = Lifetime;
            projectile.melee = true;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write((byte)projectile.direction);

        public override void ReceiveExtraAI(BinaryReader reader) => projectile.direction = reader.ReadByte();

		public override void AI()
		{
			// Determine the initial direction of the tooth in terms of its orbit.
            if (Main.myPlayer == projectile.owner && projectile.spriteDirection == 0)
			{
                projectile.spriteDirection = Main.rand.NextBool(2).ToDirectionInt();
				projectile.netUpdate = true;
			}

			projectile.rotation = MathHelper.Pi * Time / Lifetime;
			if (projectile.spriteDirection == -1)
				projectile.rotation = MathHelper.Pi - projectile.rotation;
			projectile.rotation += projectile.velocity.ToRotation();

			// In this context, the velocity is simply the initial direction as a unit vector- it does not
			// actually influence movement in any way.
			float angle = MathHelper.TwoPi * Time / Lifetime - MathHelper.PiOver2;
			if (projectile.spriteDirection == -1)
				angle += MathHelper.Pi;

			Vector2 baseDirection = angle.ToRotationVector2();
			baseDirection.X *= 0.25f;
			baseDirection.Y = baseDirection.Y * 0.5f + 0.5f;
			baseDirection = baseDirection.RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver2);

			projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter) + baseDirection * ShootReach;
			projectile.Opacity = Utils.InverseLerp(0f, 12f, Time, true) * Utils.InverseLerp(Lifetime, Lifetime - 12f, Time, true);

			Time++;
		}

		public override Color? GetAlpha(Color lightColor)
		{
            float hue = (projectile.identity % 9f / 9f + projectile.timeLeft / (float)Lifetime) % 1f;
            return Color.Lerp(lightColor, Main.hslToRgb(hue, 0.95f, 0.55f), 0.35f) * projectile.Opacity;
		}

		internal float WidthFunction(float completionRatio) => 54f;

		internal Color ColorFunction(float completionRatio)
		{
			Color color = Color.Lerp(Color.White, Color.Violet, Utils.InverseLerp(0f, 0.7f, completionRatio, true)) * (1f - Utils.InverseLerp(0f, 0.98f, completionRatio, true));
			color.A /= 2;
			return color;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (TrailDrawer is null)
				TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, false, GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"]);

			GameShaders.Misc["CalamityMod:OverpoweredTouhouSpearShader"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
			TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition + projectile.velocity.SafeNormalize(Vector2.Zero) * 86f, 60);
			return true;
		}

		public override bool ShouldUpdatePosition() => false;
	}
}
