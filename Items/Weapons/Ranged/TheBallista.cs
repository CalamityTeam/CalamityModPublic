using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheBallista : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ballista");
            Tooltip.SetDefault("Converts wooden arrows into greatarrows that crush enemy armor and break into shards on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 99;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 70;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.buyPrice(0, 80, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BallistaGreatArrow>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.WoodenArrowFriendly)
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BallistaGreatArrow>(), damage, knockback, player.whoAmI);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Marrow).
                AddIngredient(ItemID.AncientBattleArmorMaterial).
                AddIngredient(ItemID.Ectoplasm, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
