using CalamityMod.Items.Accessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class SpiritOriginBullseye : ModProjectile
    {
        public Player Owner => Main.player[projectile.owner];
        public NPC Target => Main.npc[(int)projectile.ai[0]];
        public bool FadingOut
        {
            get => projectile.ai[1] == 1f;
            set => projectile.ai[1] = value.ToInt();
        }
        public Vector2 BullseyeOffsetFromCenter;
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetDefaults()
        {
            projectile.width = projectile.height = (int)(2 * DaawnlightSpiritOrigin.RegularEnemyBullseyeRadius);
            projectile.aiStyle = -1;
            projectile.friendly = false;
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Opacity = 0f;
            projectile.penetrate = -1;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.WriteVector2(BullseyeOffsetFromCenter);

        public override void ReceiveExtraAI(BinaryReader reader) => BullseyeOffsetFromCenter = reader.ReadVector2();

        public override void AI()
        {
            if (!Main.npc.IndexInRange((int)projectile.ai[0]) || !Owner.Calamity().spiritOrigin || !Target.active || Target.life <= 0 || Target.dontTakeDamage)
            {
                projectile.Kill();
                return;
            }

            if (projectile.timeLeft < 45)
                FadingOut = true;

            projectile.Opacity = MathHelper.Clamp(projectile.Opacity + (FadingOut ? -0.08f : 0.04f), 0f, 1f);
            projectile.scale = projectile.Opacity;
            if (FadingOut && projectile.Opacity <= 0f)
                projectile.Kill();

            if (BullseyeOffsetFromCenter == Vector2.Zero)
            {
                BullseyeOffsetFromCenter = Main.rand.NextVector2CircularEdge(Target.width, Target.height) * Main.rand.NextFloat(0.925f, 1f) * 0.54f;
                if (BullseyeOffsetFromCenter.Y > 0f)
                    BullseyeOffsetFromCenter.Y *= -1f;
                projectile.netUpdate = true;
            }
            else
                projectile.Center = Target.Center + BullseyeOffsetFromCenter;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.myPlayer != projectile.owner)
                return false;

            float scale = 2f - projectile.scale;
            float rotation = MathHelper.TwoPi * projectile.scale;
            Vector2 drawPosition = Target.Center + BullseyeOffsetFromCenter - Main.screenPosition;
            if (FadingOut)
            {
                scale = projectile.scale;
                rotation = 0f;
            }

            Texture2D bullseyeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/DaawnlightSpiritOriginRegularBullseye");
            Rectangle frame = bullseyeTexture.Frame();
            if (Target.IsABoss())
            {
                bullseyeTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/DaawnlightSpiritOriginBossBullseye");
                frame = bullseyeTexture.Frame(1, 4, 0, (int)(Main.GlobalTime * 7f) % 4);
                rotation = 0f;
                drawPosition.Y -= 17;
                drawPosition.X -= 1;
            }

            spriteBatch.Draw(bullseyeTexture, drawPosition, frame, Color.White * projectile.Opacity, rotation, frame.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
