using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class CadencePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
            DisplayName.SetDefault("Cadance Potion");
            Tooltip.SetDefault("Gives the cadance buff which increases life regeneration and heart pickup range\n" +
                               "Increases max life by 25%\n" +
                                "While this potion's buff is active, Regeneration Potion and Lifeforce Potion buffs are disabled");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 38;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.UseSound = SoundID.Item3;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Cadence>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LovePotion).
                AddIngredient(ItemID.HeartreachPotion).
                AddIngredient(ItemID.LifeforcePotion).
                AddIngredient(ItemID.RegenerationPotion).
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(40).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
