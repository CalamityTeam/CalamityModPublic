using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class AscendantSpiritEssence : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ascendant Spirit Essence");
            Tooltip.SetDefault("A catalyst of the highest caliber formed by fusing powerful souls");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 54;
            item.maxStack = 999;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = Item.sellPrice(gold: 40);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = Main.itemTexture[item.type];
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Materials/AscendantSpiritEssenceGlow");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, 6, 6, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            float brightness = Main.essScale * Main.rand.NextFloat(0.9f, 1.1f);
            Lighting.AddLight(item.Center, 1.2f * brightness, 0.4f * brightness, 0.8f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 2);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
