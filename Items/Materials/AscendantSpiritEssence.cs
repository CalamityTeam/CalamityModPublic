using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class AscendantSpiritEssence : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            DisplayName.SetDefault("Ascendant Spirit Essence");
            Tooltip.SetDefault("A catalyst of the highest caliber formed by fusing powerful souls");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.value = Item.sellPrice(gold: 40);
        }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = TextureAssets.Item[Item.type].Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Materials/AscendantSpiritEssenceGlow").Value;
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(Item.Center, 1.2f * brightness, 0.4f * brightness, 0.8f);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Phantoplasm>(2).
                AddIngredient<NightmareFuel>(5).
                AddIngredient<EndothermicEnergy>(5).
                AddIngredient<DarksunFragment>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
