using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GreatswordofBlah : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatsword of Blah");
            Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
                               "You can hear faint yet comforting whispers emanating from the blade\n" +
                               "'No matter where you may be you are never alone\n" +
                               "I shall always be at your side, my lord'\n" +
                               "Fires a rainbow blade that emits rainbow rain on death for a time");
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 102;
            Item.damage = 138;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.shoot = ModContent.ProjectileType<JudgementBlah>();
            Item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GreatswordofJudgement>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
