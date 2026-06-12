using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class TheAmalgam : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";

        public static int NimbusDamage => CalamityUtils.ScaleWithDifficulty(200);

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrainRot>(), ModContent.BuffType<BrimstoneFlames>(), ModContent.BuffType<CrushDepth>(), ModContent.BuffType<Plague>(), BuffID.Electrified, BuffID.Frostburn2];
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = RarityType<CosmicPurple>();
            Item.expert = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rBrain = true; // Handles shaderain cloud spawning on hit
            modPlayer.amalgam = true;
            player.brainOfConfusionItem = Item;
            modPlayer.HeatDebuffMultiplier += 2f;
            modPlayer.ColdDebuffMultiplier += 2f;
            modPlayer.SicknessDebuffMultiplier += 2f;
            modPlayer.WaterDebuffMultiplier += 2f;
            modPlayer.ElectricDebuffMultiplier += 2f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AmalgamatedBrain>().
                AddIngredient<AscendantSpiritEssence>(4).
                AddIngredient<AshesofCalamity>(12).
                AddIngredient<EssenceofEleum>(15).
                AddIngredient<PlagueCellCanister>(15).
                AddIngredient<DepthCells>(15).
                AddIngredient<ArmoredShell>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
