using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class StarTaintedGenerator : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Star-Tainted Generator");
            Tooltip.SetDefault("+2 max minions, does not stack with downgrades\n" +
                    "7% increased minion damage\n" +
                    "Minion attacks spawn astral explosions and inflict several debuffs");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 22;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().voltaicJelly = true;
            player.Calamity().starbusterCore = true;
            player.Calamity().starTaintedGenerator = true;
            player.GetDamage(DamageClass.Summon) += 0.07f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<JellyChargedBattery>().
                AddIngredient<NuclearRod>().
                AddIngredient<StarbusterCore>().
                AddIngredient<BarofLife>(3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
