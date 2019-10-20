using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.NPCs;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.SummonItems
{
    public class ProfanedShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Shard");
            Tooltip.SetDefault("A shard of the unholy flame\n" +
                "Summons the Profaned Guardians\n" +
                "Can only be used during daytime");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.consumable = true;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<ProfanedGuardianBoss>()) && Main.dayTime && (player.ZoneHoly || player.ZoneUnderworldHeight);
        }

        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<ProfanedGuardianBoss>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 15);
            recipe.AddIngredient(ItemID.LunarBar, 3);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
