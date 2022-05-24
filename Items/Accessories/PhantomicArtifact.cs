using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    class PhantomicArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Phantomic Artifact");
            Tooltip.SetDefault("Whenever your minions hit an enemy you will gain a random phantomic buff, does not stack with downgrades\n" +
                "These buffs will either boost your defense, summon damage, or life regen for a while\n" +
                "If you have the offensive boost, enemies hit by minions will sometimes be hit by phantomic knives\n" +
                "If you have the regenerative boost, a phantomic heart will occasionally materialise granting massive health regen\n" +
                "If you have the defensive boost, a phantomic bulwark will absorb 20% of the next projectile's damage that hits the bulwark, shattering it");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 7));
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().phantomicArtifact = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HallowedRune>().
                AddIngredient<RuinousSoul>(5).
                AddIngredient<BloodOrb>(10).
                AddIngredient<ExodiumClusterOre>(20).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
