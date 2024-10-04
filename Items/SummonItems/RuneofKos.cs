using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.Rarities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    [LegacyName("RuneofCos")]
    public class RuneofKos : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public static readonly SoundStyle CVSound = new("CalamityMod/Sounds/Item/CeaselessVoidSpawn");
        public static readonly SoundStyle SignutSound = new("CalamityMod/Sounds/Item/SignusSpawn");
        public static readonly SoundStyle StormSound = new("CalamityMod/Sounds/Item/StormWeaverSpawn");
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = null;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return (player.ZoneSkyHeight || player.ZoneUnderworldHeight || player.ZoneDungeon) &&
                !NPC.AnyNPCs(ModContent.NPCType<StormWeaverHead>()) && !NPC.AnyNPCs(ModContent.NPCType<CeaselessVoid>()) && !NPC.AnyNPCs(ModContent.NPCType<Signus>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            if (player.ZoneDungeon)
            {
                CalamityUtils.SpawnBossUsingItem<CeaselessVoid>(player, CVSound);
            }
            else if (player.ZoneUnderworldHeight)
            {
                CalamityUtils.SpawnBossUsingItem<Signus>(player, SignutSound);
            }
            else if (player.ZoneSkyHeight)
            {
                CalamityUtils.SpawnBossUsingItem<StormWeaverHead>(player, StormSound);
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.LocalPlayer;
            string line = this.GetLocalizedValue("SpawnInfo");
            if (player.ZoneDungeon)
                line = this.GetLocalizedValue("SpawnVoid");
            else if (player.ZoneUnderworldHeight)
                line = this.GetLocalizedValue("SpawnSignus");
            else if (player.ZoneSkyHeight)
                line = this.GetLocalizedValue("SpawnWeaver");
            list.FindAndReplace("[SPAWN]", line);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 3).
                AddIngredient<UnholyEssence>(40).
                AddIngredient(ItemID.FragmentSolar, 5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
