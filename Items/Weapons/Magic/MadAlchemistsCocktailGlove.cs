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
        private int FlaskType = 0;
        private int BaseDamage = 150;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Glove");
            Tooltip.SetDefault("Fires a variety of high-velocity flasks that have various effects\n" +
                "Right click to throw a flask that inflicts a variety of debuffs");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
            item.noUseGraphic = true;
            item.mana = 12;
            item.width = 26;
            item.height = 36;
            item.useTime = 15;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = 11;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MadAlchemistsCocktailRed>();
            item.shootSpeed = 12f;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player) => base.CanUseItem(player);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<MadAlchemistsCocktailAlt>();
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)(damage * 1.15f), knockBack, player.whoAmI, 0f, 0f);
            }
            else
            {
                switch (FlaskType)
                {
                    case 0:
                        type = ModContent.ProjectileType<MadAlchemistsCocktailRed>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<MadAlchemistsCocktailBlue>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<MadAlchemistsCocktailGreen>();
                        break;
                    case 3:
                        type = ModContent.ProjectileType<MadAlchemistsCocktailPurple>();
                        break;
                    default:
                        break;
                }

                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);

                FlaskType++;
                if (FlaskType > 3)
                    FlaskType = 0;
            }

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.ToxicFlask);
            recipe.AddIngredient(ItemID.BottledWater, 15);
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(ModContent.ItemType<EffulgentFeather>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCinder>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
