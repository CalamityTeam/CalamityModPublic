using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class ProfanedRagePotion : ModItem
    {
        internal static readonly int CritBoost = 12;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Profaned Rage Potion");
            Tooltip.SetDefault("Increases critical strike chance by 12%\n" +
                "While this potion's buff is active the Rage Potion's buff is disabled");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 42;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Purple;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ProfanedRageBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(300f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            // as an upgrade to an already-existing rage potion (e.g. purchased)
            CreateRecipe().
                AddIngredient(ItemID.RagePotion).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AlchemyTable).
                AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy).
                Register();

            // rage potion + 1 unholy essence directly
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient(ItemID.Hemopiranha).
                AddIngredient(ItemID.Deathweed).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AlchemyTable).
                AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy).
                Register();

            // blood orbs
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(40).
                AddIngredient<UnholyEssence>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
