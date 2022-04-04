using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Zapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lazinator");
            Tooltip.SetDefault("Zap");
        }

        public override void SetDefaults()
        {
            Item.damage = 55;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 46;
            Item.height = 22;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 48, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurpleLaser;
            Item.shootSpeed = 20f;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            if (Main.rand.NextBool(2))
            {
                type = ProjectileID.GreenLaser;
            }
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int laser = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            Main.projectile[laser].usesLocalNPCImmunity = true;
            Main.projectile[laser].localNPCHitCooldown = 10;
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.SpaceGun).AddIngredient(ItemID.LaserRifle).AddIngredient(ModContent.ItemType<VictoryShard>(), 5).AddIngredient(ItemID.SoulofSight).AddIngredient(ItemID.SoulofMight).AddIngredient(ItemID.SoulofFright).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
