using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class CosmicWorm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 19; // Celestial Sigil
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            // Server literally do nothing here.
            if (Main.dedServ)
                return true;

            NPC n = CalamityUtils.SpawnBossOnPosUsingItem<DevourerofGodsHead>(player, (int)player.Center.X, (int)player.Center.Y - 1600, DevourerofGodsHead.SpawnSound);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArmoredShell>().
                AddIngredient<TwistingNether>().
                AddIngredient<DarkPlasma>().
                AddTile(TileID.MythrilAnvil).
                Register()
                .DisableDecraft();

            // sequence breaking recipe
            CreateRecipe().
                AddIngredient(ItemID.LunarBar, 40).
                AddIngredient(ItemID.FragmentSolar, 10).
                AddIngredient(ItemID.FragmentVortex, 10).
                AddIngredient(ItemID.FragmentNebula, 10).
                AddIngredient(ItemID.FragmentStardust, 10).
                AddIngredient<MeldBlob>(10).
                AddIngredient<Necroplasm>(40).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
