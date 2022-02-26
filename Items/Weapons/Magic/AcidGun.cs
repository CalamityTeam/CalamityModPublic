using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AcidGun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Gun");
            Tooltip.SetDefault("Releases three streams of acid");
        }

        public override void SetDefaults()
        {
            item.damage = 17;
            item.magic = true;
            item.mana = 9;
            item.width = 42;
            item.height = 28;
            item.useTime = item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item13;
            item.autoReuse = true;
            item.shootSpeed = 14f;
            item.shoot = ModContent.ProjectileType<AcidGunStream>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 3; i++)
            {
                float angle = MathHelper.Lerp(-0.145f, 0.145f, i / 3f);
                Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedBy(angle), type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Acidwood>(), 35);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
