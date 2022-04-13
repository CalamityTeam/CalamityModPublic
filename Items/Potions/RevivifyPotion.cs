using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class RevivifyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Revivify Potion");
            Tooltip.SetDefault("Causes enemy attacks to heal you for a fraction of their damage");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Revivify>();
            Item.buffTime = CalamityUtils.SecondsToFrames(180f);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe(5).
                AddIngredient(ItemID.HolyWater, 5).
                AddIngredient<Stardust>(20).
                AddIngredient(ItemID.CrystalShard, 5).
                AddIngredient<EssenceofCinder>(3).
                AddIngredient<ScarredAngelfish>().
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe(5).
                AddIngredient(ItemID.HolyWater, 5).
                AddIngredient<BloodOrb>(100).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
