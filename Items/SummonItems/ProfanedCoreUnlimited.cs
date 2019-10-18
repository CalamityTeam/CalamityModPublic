using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
namespace CalamityMod.Items.SummonItems
{
    public class ProfanedCoreUnlimited : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Core");
            Tooltip.SetDefault("The core of the unholy flame\n" +
                "Summons Providence\n" +
                "Can only be used during daytime\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = false;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Providence>()) && Main.dayTime && (player.ZoneHoly || player.ZoneUnderworldHeight) && CalamityWorld.downedBossAny;
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Providence>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ProfanedCore");
            recipe.AddIngredient(null, "UnholyEssence", 50);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
