using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Miasma : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miasma");
            Tooltip.SetDefault("Fires a spread of gas clouds that slow down after hitting an enemy");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 50;
            Item.height = 64;
            Item.useTime = Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MiasmaGas>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			int cloudAmt = Main.rand.Next(3, 5 + 1);
            for (int i = 0; i < cloudAmt; i++)
            {
                Vector2 velocityReal = velocity * Main.rand.NextFloat(0.9f, 1.1f);
                float angle = Main.rand.NextFloat(-1f, 1f) * MathHelper.ToRadians(30f);
                Projectile.NewProjectile(source, position, velocityReal.RotatedBy(angle), type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.NimbusRod).AddIngredient(ModContent.ItemType<AquamarineStaff>()).AddIngredient(ModContent.ItemType<CorrodedFossil>(), 10).AddTile(TileID.Anvils).Register();
        }
    }
}
