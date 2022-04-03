using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ThePrince : ModItem
    {
        public const int FlameSplitCount = 6;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Prince");
            Tooltip.SetDefault($"Casts a holy fireball that explodes into {FlameSplitCount} flames\n" +
                               "So you're telling me that the prince exploded, and then turned into a flower?\n" +
                               "-Dain, the sailor druid");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 166;
            Item.knockBack = 4.25f;
            Item.shootSpeed = 23.5f;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.mana = 12;
            Item.width = 102;
            Item.height = 112;
            Item.useTime = Item.useAnimation = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.DD2_FlameburstTowerShot;
            Item.shoot = ModContent.ProjectileType<PrinceFlameLarge>();
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().donorItem = true;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(20f, 20f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 velocity = new Vector2(speedX, speedY);
            Vector2 flameSpawnPosition = player.RotatedRelativePoint(player.MountedCenter, true);
            flameSpawnPosition += velocity.SafeNormalize(Vector2.Zero) * 105f;
            Projectile.NewProjectile(flameSpawnPosition, velocity, type, damage, knockBack, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ArchAmaryllis>()).AddIngredient(ModContent.ItemType<DivineGeode>(), 15).AddIngredient(ModContent.ItemType<UnholyEssence>(), 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
