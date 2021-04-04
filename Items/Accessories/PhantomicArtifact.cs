using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;

namespace CalamityMod.Items.Accessories
{
    class PhantomicArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantomic Artifact");
            Tooltip.SetDefault("Whenever your minions hit an enemy you will gain a random phantomic buff, does not stack with downgrades\n" +
                "These buffs will either boost your defense, summon damage, or life regen for a while\n" +
                "If you have the offensive boost, enemies hit by minions will sometimes be hit by phantomic knives\n" +
                "If you have the regenerative boost, a phantomic heart will occasionally materialise granting massive health regen\n" +
                "If you have the defensive boost, a phantomic bulwark will absorb 20% of the next projectile's damage that hits the bulwark, shattering it");
        }

        public override void SetDefaults()
        {
            item.width = 48;
            item.height = 62;
            item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            item.rare = ItemRarityID.Red; 
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().phantomicArtifact = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<HallowedRune>());
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
