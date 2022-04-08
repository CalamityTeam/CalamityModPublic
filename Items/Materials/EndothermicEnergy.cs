using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;

namespace CalamityMod.Items.Materials
{
    public class EndothermicEnergy : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endothermic Energy");
            Tooltip.SetDefault("Its deathly chill sucks the life from its surroundings");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(gold: 2);
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/EndothermicEnergyGlow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0f, 0f, 1.2f * brightness);
        }
    }
}
