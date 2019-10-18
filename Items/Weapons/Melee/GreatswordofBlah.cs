using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GreatswordofBlah : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatsword of Blah");
            Tooltip.SetDefault("A pale white sword from a forgotten land\n" +
                                "Fires a rainbow orb that emits rainbow rain on death for a time\n" +
                               "You can hear faint yet comforting whispers emanating from the blade\n" +
                                "'No matter where you may be you are never alone\n" +
                                "I shall always be at your side, my lord'");
        }

        public override void SetDefaults()
        {
            item.width = 110;
            item.damage = 480;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 110;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<JudgementBlah>();
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "GreatswordofJudgement");
            recipe.AddIngredient(null, "NightmareFuel", 10);
            recipe.AddIngredient(null, "EndothermicEnergy", 10);
            recipe.AddIngredient(null, "DarksunFragment", 10);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
