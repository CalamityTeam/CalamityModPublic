using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ShadowspecBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowspec Bar");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(8, 10));
        }

        public override void SetDefaults()
        {
            item.width = 15;
            item.height = 12;
            item.maxStack = 999;
            item.rare = 10;
            item.value = Item.sellPrice(gold: 27, silver: 50);
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BarofLife", 3);
            recipe.AddIngredient(null, "Phantoplasm", 3);
            recipe.AddIngredient(null, "NightmareFuel", 3);
            recipe.AddIngredient(null, "EndothermicEnergy", 3);
            recipe.AddIngredient(null, "CalamitousEssence");
            recipe.AddIngredient(null, "DarksunFragment");
            recipe.AddIngredient(null, "HellcasterFragment");
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
        }
    }
}
