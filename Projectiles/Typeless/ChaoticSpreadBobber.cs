using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
    public class ChaoticSpreadBobber : ModProjectile
    {
        private bool initialized = false;
        private Color fishingLineColor;
        public Color[] PossibleLineColors = new Color[]
        {
            new Color(255, 165, 0, 100), //an orange color
            new Color(0, 206, 209, 100) // a blue color
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaotic Bobber");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.aiStyle = 61;
            projectile.bobber = true;
            projectile.penetrate = -1;
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

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            if (fishingLineColor == PossibleLineColors[0])
                Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);
            else
                Lighting.AddLight(projectile.Center, 0f, 0.45f, 0.46f);
            return projectile.DrawFishingLine(ModContent.ItemType<ChaoticSpreadRod>(), fishingLineColor, 65, 30f);
        }
    }
}
