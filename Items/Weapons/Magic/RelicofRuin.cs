using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class RelicofRuin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Ruin");
            Tooltip.SetDefault("Casts a spread of sand blades");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.magic = true;
            item.mana = 16;
            item.width = 34;
            item.height = 40;
            item.useTime = 35;
            item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item84;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ForbiddenAxeBlade>();
            item.shootSpeed = 5f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int totalProjectiles = 12;
            float radians = MathHelper.TwoPi / totalProjectiles;
            for (int i = 0; i < totalProjectiles; i++)
            {
                Vector2 vector = new Vector2(0f, -item.shootSpeed).RotatedBy(radians * i);
                Projectile.NewProjectile(position, vector, type, damage, knockBack, Main.myPlayer);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpellTome);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 5); // This is here to keep the Forbidden Fragment stuff on the same tier.
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddTile(TileID.Bookcases);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
