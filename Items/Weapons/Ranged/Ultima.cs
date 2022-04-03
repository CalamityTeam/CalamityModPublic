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
            Item.damage = 116;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 44;
            Item.height = 58;
            Item.useTime = Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<UltimaBowProjectile>();
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
            Item.channel = true;
            Item.useTurn = false;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<UltimaBowProjectile>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override bool ConsumeAmmo(Player player) => Main.rand.Next(0, 100) >= 90;
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.PulseBow).AddIngredient(ItemID.LaserRifle).AddIngredient(ModContent.ItemType<TheStorm>()).AddIngredient(ModContent.ItemType<AstralRepeater>()).AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 15).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<DarksunFragment>(), 8).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
