using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.SummonItems
{
    public class Starcore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starcore");
            Tooltip.SetDefault("May the stars guide your way\n" +
                "Summons Astrum Deus\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = 9;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHead>()) && !NPC.AnyNPCs(ModContent.NPCType<AstrumDeusHeadSpectral>());
        }

        public override bool UseItem(Player player)
        {
            for (int x = 0; x < 10; x++)
            {
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AstrumDeusHead>());
            }
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<AstrumDeusHeadSpectral>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 25);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 4);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
