using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public class SlashThrough : Particle
    {
        public override string Texture => "CalamityMod/ExtraTextures/Trails/SwordSlashTexture";
        public override bool UseAdditiveBlend => true;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => true;

        private Vector2 OriginalPosition;
        private Vector2 CurrentScale;
        private Color BaseColor;
        public NPC StuckTo;

        public SlashThrough(Color color, Vector2 originalPosition, float rotation, int lifeTime, NPC stuckTo)
        {
            BaseColor = color;
            OriginalPosition = originalPosition;
            Position = originalPosition;
            Rotation = rotation;
            Lifetime = lifeTime;
            StuckTo = stuckTo;
        }

        public override void Update()
        {
            float opacity = (float)Math.Sin(LifetimeCompletion * MathHelper.Pi) * 0.8f + 0.2f;
            Color = BaseColor * opacity;
            CurrentScale = new Vector2(128f, 64f);

            if (StuckTo != null && StuckTo.active && StuckTo.knockBackResist > 0f)
            {
                float distance = Vector2.Distance(OriginalPosition, StuckTo.Center);
                Position = Vector2.Lerp(OriginalPosition, StuckTo.Center, StuckTo.knockBackResist);
                CurrentScale.X += distance;
                if (distance > 4f)
                    Rotation = StuckTo.SafeDirectionTo(OriginalPosition).ToRotation() + MathHelper.Pi;
            }
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color, Rotation, tex.Size() / 2f, CurrentScale / 256f, SpriteEffects.None, 0);
        }
    }
}
