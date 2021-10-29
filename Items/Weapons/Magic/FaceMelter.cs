using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FaceMelter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Face Melter");
            Tooltip.SetDefault("WOOO!! FAAAAAAANTASYY WORLDDDDD!\n" +
                "Fires music notes\n" +
                "Right click summons an amplifier that shoots towards your mouse");
        }

        public override void SetDefaults()
        {
            item.damage = 140;
            item.magic = true;
            item.mana = 10;
            item.width = 56;
            item.height = 50;
            item.useTime = 5;
            item.useAnimation = 10;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;

            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;

            item.shoot = ModContent.ProjectileType<MelterNote1>();
            item.UseSound = SoundID.Item47;
            item.autoReuse = true;
            item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TheAxe);
            recipe.AddIngredient(ItemID.MagicalHarp);
            recipe.AddIngredient(ModContent.ItemType<SirensSong>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 20);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 20;
                item.useAnimation = 20;
            }
            else
            {
                item.useTime = 5;
                item.useAnimation = 10;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MelterAmp>(), damage, knockBack, player.whoAmI);
                return false;
            }
            else
            {
                int note = Main.rand.Next(2);
                if (note == 0)
                {
                    damage = (int)(damage * 1.5f);
                    type = ModContent.ProjectileType<MelterNote1>();
                }
                else
                {
                    speedX *= 1.5f;
                    speedY *= 1.5f;
                    type = ModContent.ProjectileType<MelterNote2>();
                }
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
                return false;
            }
        }
    }
}
