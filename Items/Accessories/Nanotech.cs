using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Nanotech : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Nanotech");
            Tooltip.SetDefault("Rogue projectiles create nanoblades as they travel\n" +
                "Stealth strikes summon nanobeams and sparks on enemy hits\n" +
                "Stealth strikes have +15 armor penetration, deal 5% more damage, and heal for 2 HP\n" +
                "15% increased rogue damage and 15% increased rogue velocity\n" +
                $"Landing a stealth strike grants a {(int)RaidersTalisman.RaiderBonus}% crit bonus to non-stealth strikes\n" +
                $"This crit bonus decays over {RaidersTalisman.RaiderCooldown} seconds");
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 46;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.nanotech = true;
            modPlayer.raiderTalisman = true;
            modPlayer.electricianGlove = true;
            modPlayer.filthyGlove = true;
            modPlayer.bloodyGlove = true;
            player.GetDamage<ThrowingDamageClass>() += 0.15f;
            player.Calamity().rogueVelocity += 0.15f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RogueEmblem>().
                AddIngredient<RaidersTalisman>().
                AddIngredient<MoonstoneCrown>().
                AddIngredient<ElectriciansGlove>().
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
