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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 166;
            item.knockBack = 4.25f;
            item.shootSpeed = 23.5f;
            item.magic = true;
            item.noMelee = true;
            item.mana = 12;
            item.width = 102;
            item.height = 112;
            item.useTime = item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = true;
            item.UseSound = SoundID.DD2_FlameburstTowerShot;
            item.shoot = ModContent.ProjectileType<PrinceFlameLarge>();
            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().donorItem = true;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArchAmaryllis>());
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 15);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
