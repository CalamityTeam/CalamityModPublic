using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OpalStriker : ModItem
    {
        public static readonly SoundStyle FireSound = new("CalamityMod/Sounds/Item/OpalStrike");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Opal Striker");
            Tooltip.SetDefault("Fires a string of opal strikes");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 46;
            Item.height = 30;
            Item.useTime = 5;
            Item.reuseDelay = 25;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = FireSound;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<OpalStrike>();
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<OpalStrike>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Marble, 20).
                AddIngredient(ItemID.Amber, 5).
                AddIngredient(ItemID.Diamond, 3).
                AddIngredient(ItemID.FlintlockPistol).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
