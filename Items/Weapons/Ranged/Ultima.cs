using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Ultima : ModItem
    {
        public const float FullChargeTime = 420f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultima");
            Tooltip.SetDefault("Casts a continuous stream of plasma bolts\n" +
                               "Over time the bolts are replaced with powerful lasers\n" +
                               "Bolts power up into solid beams as you continue shooting\n" +
                               "90% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 119;
            item.ranged = true;
            item.width = 44;
            item.height = 58;
            item.useTime = item.useAnimation = 3;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.rare = 10;
            item.UseSound = SoundID.Item33;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<UltimaBowProjectile>();
            item.shootSpeed = 18f;
            item.useAmmo = AmmoID.Arrow;
            item.channel = true;
            item.useTurn = false;
            item.autoReuse = true;
            item.noUseGraphic = true;
            item.Calamity().customRarity = CalamityRarity.Dedicated;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<UltimaBowProjectile>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(0, 100) >= 90;
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PulseBow);
            recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(ModContent.ItemType<TheStorm>());
            recipe.AddIngredient(ModContent.ItemType<AstralRepeater>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 15);
            recipe.AddIngredient(ModContent.ItemType<DarksunFragment>(), 15);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
