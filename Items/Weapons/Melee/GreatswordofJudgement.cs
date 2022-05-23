using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GreatswordofJudgement : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatsword of Judgement");
            Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
                               "You can hear faint yet comforting whispers emanating from the blade\n" +
                               "'No matter where you may be you are never alone\n" +
                               "I shall always be at your side, my lord'\n" +
                               "Fires a white orb that emits white rain on death for a time");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 78;
            Item.damage = 48;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 78;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<JudgementProj>();
            Item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 7).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
