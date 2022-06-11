using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("DivineHatchet")]
    public class SeekingScorcher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seeking Scorcher");
            Tooltip.SetDefault("May your enemies burn in hell for the sins they have committed\n" +
            "Throws a holy boomerang that seeks out up to three enemies before returning to the player");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.damage = 177;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 17;
            Item.knockBack = 8.5f;
            Item.UseSound = SoundID.Item1;
            Item.DamageType = DamageClass.Melee;
            Item.height = 62;
            Item.shoot = ModContent.ProjectileType<DivineHatchetBoomerang>();
            Item.shootSpeed = 14f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PossessedHatchet).
                AddIngredient<DivineGeode>(5).
                AddIngredient<UelibloomBar>(9).
                AddIngredient<UnholyEssence>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
