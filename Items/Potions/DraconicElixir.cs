using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class DraconicElixir : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draconic Elixir");
            Tooltip.SetDefault("Greatly increases wing flight time and speed and increases defense by 16\n" +
                "Silva invincibility heals you to half HP when triggered\n" +
                "If you trigger the above heal you cannot drink this potion again for 60 seconds and you gain 30 seconds of potion sickness");
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 44;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<DraconicSurgeBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override bool CanUseItem(Player player) => !player.HasCooldown(Cooldowns.DraconicElixir.ID);

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Potions/DraconicElixir_Animated");
            spriteBatch.Draw(texture, position, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 10), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Potions/DraconicElixir_Animated");
            spriteBatch.Draw(texture, Item.position - Main.screenPosition, Item.GetCurrentFrame(ref frame, ref frameCounter, 8, 10), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<HellcasterFragment>()).AddIngredient(ItemID.Daybloom).AddIngredient(ItemID.Moonglow).AddIngredient(ItemID.Fireblossom).AddTile(TileID.AlchemyTable).Register();
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<BloodOrb>(), 50).AddIngredient(ModContent.ItemType<HellcasterFragment>()).AddTile(TileID.AlchemyTable).Register();
        }
    }
}
