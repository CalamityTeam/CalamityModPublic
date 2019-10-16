using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BloodIdol : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Relic");
            Tooltip.SetDefault("Summons a blood moon");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = 4;
            item.UseSound = SoundID.Item66;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.bloodMoon && !Main.dayTime;
        }

        public override bool UseItem(Player player)
        {
            Main.bloodMoon = true;
            CalamityMod.UpdateServerBoolean();
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodlettingEssence", 2);
            recipe.AddIngredient(null, "UnholyCore", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "FetidEssence", 2);
            recipe.AddIngredient(null, "UnholyCore", 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
