using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RustyMedallion : ModItem
    {
        public const int AcidDropBaseDamage = 9;

        // BAD Minishark! You will NOT become stupid sleeper agent item!
        public const int AcidCreationCooldown = 25;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Rusty Medallion");
            Tooltip.SetDefault("Causes most ranged weapons to release acid droplets from the sky\n" +
                "Grants immunity to Irradiated");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.RustyMedallionDroplets = true;
            player.buffImmune[ModContent.BuffType<Irradiated>()] = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SulphuricScale>(20).
                AddRecipeGroup("AnySilverBar", 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
