using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.GameContent;

namespace CalamityMod.Items.Materials
{
    public class DarksunFragment : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Darksun Fragment");
            Tooltip.SetDefault("An impacted crystal suffused with opposing celestial energies");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.sellPrice(gold: 12);
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 8), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/DarksunFragmentGlow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 8, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 0.5f * brightness, 0.5f * brightness, 0.5f * brightness);
        }
    }
}
