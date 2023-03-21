using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace CalamityMod.Items.Accessories
{
    public class DarkSunRing : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Dark Sun Ring");
            Tooltip.SetDefault("Contains the power of the dark sun\n" +
                "12% increase to damage and melee speed\n" +
                "+1 life regen, 15% increased mining speed and +2 max minions\n" +
                "Increased minion knockback\n" +
                "During the day the player has +4 life regen\n" +
                "During the night the player has +20 defense\n" +
                "During an eclipse the player has +2 life regen and +10 defense");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 7));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 60;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.defense = 10;
            Item.lifeRegen = 1;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.darkSunRing = true;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UelibloomBar>(10).
                AddIngredient<DarksunFragment>(20).
                AddTile<CosmicAnvil>().
                Register();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            CalamityUtils.DrawInventoryCustomScale(
                spriteBatch,
                texture: TextureAssets.Item[Type].Value,
                position,
                frame,
                drawColor,
                itemColor,
                origin,
                scale,
                wantedScale: 0.8f,
                drawOffset: new(0f, 0f)
            );
            return false;
        }
    }
}
