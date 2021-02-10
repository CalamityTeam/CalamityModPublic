using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class MadAlchemistsCocktailGlove : ModItem
    {
        private const int BaseDamage = 146;
        private int flaskIndex = 0;

        private static int[] flaskIDs;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Glove");
            Tooltip.SetDefault("Fires a variety of high-velocity flasks\n" +
                "Right click to throw a prismatic flask that inflicts many debuffs\n" +
                "Red flasks explode violently, blue flasks contain poison gas,\n" +
                "green flasks summon lunar flares and purple flasks explode into homing shrapnel");

            flaskIDs = new int[]
            {
                ModContent.ProjectileType<MadAlchemistsCocktailRed>(),
                ModContent.ProjectileType<MadAlchemistsCocktailBlue>(),
                ModContent.ProjectileType<MadAlchemistsCocktailGreen>(),
                ModContent.ProjectileType<MadAlchemistsCocktailPurple>(),
                ModContent.ProjectileType<MadAlchemistsCocktailAlt>()
            };
        }

        // Rest in peace Mad Cock, you will not be missed.
        // Ozzatron 09FEB2021
        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 36;

            item.damage = BaseDamage;
            item.magic = true;
            item.noUseGraphic = true;
            item.mana = 12;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;

            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MadAlchemistsCocktailRed>();
            item.shootSpeed = 12f;

            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().donorItem = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                type = flaskIDs[4];
                return true;
            }

            // Cycle through the flask types in a circle.
            type = flaskIDs[flaskIndex++];
            if (flaskIndex > 3)
                flaskIndex = 0;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ToxicFlask);
            recipe.AddIngredient(ItemID.BottledWater, 15);
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            // the recipe previously also used the individual cores for some reason
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 2);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
