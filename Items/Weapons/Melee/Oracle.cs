using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Oracle : ModItem
    {
        public const int YoyoBaseDamage = 170;
        public const int AuraBaseDamage = 100;
        public const int AuraMaxDamage = 220;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Oracle");
            Tooltip.SetDefault("Gaze into the past, the present, the future... and the circumstances of your inevitable demise\n" +
                "Emits an aura of red lightning which charges up when hitting enemies\n" +
                "Fires auric orbs when supercharged\n" +
                "An exceptionally agile yoyo\n");

            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 50;
            Item.DamageType = DamageClass.Melee;
            Item.damage = YoyoBaseDamage;
            Item.knockBack = 4f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<OracleYoyo>();
            Item.shootSpeed = 16f;

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SolarFlare>().
                AddIngredient<TheObliterator>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
