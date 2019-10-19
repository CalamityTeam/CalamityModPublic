using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Cosmolight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmolight");
            Tooltip.SetDefault("Changes night to day and vice versa");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = 4;
            item.UseSound = SoundID.Item60;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !CalamityGlobalNPC.AnyBossNPCS();
        }

        public override bool UseItem(Player player)
        {
            Main.time = 0.0;
            Main.dayTime = !Main.dayTime;
            if (Main.dayTime)
            {
                if (++Main.moonPhase >= 8)
                {
                    Main.moonPhase = 0;
                }
            }
            CalamityMod.UpdateServerBoolean();
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Daylight>());
            recipe.AddIngredient(ModContent.ItemType<Moonlight>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
