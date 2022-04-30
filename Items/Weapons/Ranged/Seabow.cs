using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seabow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seabow");
            Tooltip.SetDefault("Fires slow-moving water blasts");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 46;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + (float)Main.rand.Next(-30, 31) * 0.05f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-30, 31) * 0.05f;
            Projectile.NewProjectile(source, position.X, position.Y, SpeedX * 0.5f, SpeedY * 0.5f, ModContent.ProjectileType<SandWater>(), (int)(damage * 0.4), 0f, player.whoAmI);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictideBar>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
