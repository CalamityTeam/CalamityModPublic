using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.GreatSandShark;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class SandstormsCore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 13; // Frost Legion
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneDesert && !(Main.zenithWorld && !player.Calamity().ZoneAstral) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark>());
        }

        public override bool? UseItem(Player player)
        {
            CalamityUtils.SpawnBossUsingItem<GreatSandShark>(player, SoundID.Roar);
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list) => list.FindAndReplace("[BIOME]", Main.zenithWorld ? CalamityUtils.GetTextValue("Biomes.AstralDesert") : Language.GetTextValue("Bestiary_Biomes.Desert"));

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.AncientBattleArmorMaterial, 3).
                AddIngredient<CoreofSunlight>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
