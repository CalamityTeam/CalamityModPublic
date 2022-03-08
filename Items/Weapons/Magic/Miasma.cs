using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Miasma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miasma");
            Tooltip.SetDefault("Fires a spread of gas clouds that slow down after hitting an enemy");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 16;
            item.width = 50;
            item.height = 64;
            item.useTime = item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 36, 0, 0);
			item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MiasmaGas>();
            item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < Main.rand.Next(3, 5 + 1); i++)
            {
                Vector2 velocity = new Vector2(speedX, speedY) * Main.rand.NextFloat(0.9f, 1.1f);
                float angle = Main.rand.NextFloat(-1f, 1f) * MathHelper.ToRadians(30f);
                Projectile.NewProjectile(position, velocity.RotatedBy(angle), type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.NimbusRod);
            recipe.AddIngredient(ModContent.ItemType<AquamarineStaff>());
            recipe.AddIngredient(ModContent.ItemType<CorrodedFossil>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
