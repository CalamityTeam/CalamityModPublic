using CalamityMod.Items.Fishing.FishingRods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DevourerofCodsBobber : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        private bool initialized = false;
        private Color fishingLineColor;
        public Color[] PossibleLineColors = new Color[]
        {
            new Color(252, 109, 202, 100), //a pink color
            new Color(39, 151, 171, 100) // a blue color
        };

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = ProjAIStyleID.Bobber;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        //What if we want to randomize the line color
        public override void AI()
        {
            if (!initialized)
            {
                //Decide color of the pole by randomizing the array
                fishingLineColor = Main.rand.Next(PossibleLineColors);
                initialized = true;
            }
        }

        //fuck glowmasks btw
        //i second this notion -Dominic
        public override void PostDraw(Color lightColor)
        {
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/DevourerofCodsGlow").Value;
            float xOffset = (glowmask.Width - Projectile.width) * 0.5f + Projectile.width * 0.5f;
            Vector2 drawPos = Projectile.position - Main.screenPosition;
            drawPos.X += xOffset;
            drawPos.Y += Projectile.height / 2f + Projectile.gfxOffY;
            Rectangle frame = new(0, 0, glowmask.Width, glowmask.Height);
            Vector2 origin = new Vector2(xOffset, Projectile.height / 2f);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Projectile.ai[0] <= 1f)
            {
                Main.spriteBatch.Draw(glowmask, drawPos, frame, Color.White, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            }
        }

        public override bool PreDrawExtras()
        {
            Lighting.AddLight(Projectile.Center, 0.35f, 0f, 0.25f);
            return Projectile.DrawFishingLine(ModContent.ItemType<TheDevourerofCods>(), fishingLineColor);
        }
    }
}
