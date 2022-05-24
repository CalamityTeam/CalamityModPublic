using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class MarkedMagnum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marked Magnum");
            Tooltip.SetDefault("Shots reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = AverageDamageClass.Instance;
            Item.damage = 4;
            Item.width = 54;
            Item.height = 20;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = false;
            Item.shootSpeed = 12f;
            Item.shoot = ModContent.ProjectileType<MarkRound>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 7).
                AddIngredient(ItemID.Obsidian, 15).
                AddIngredient(ItemID.GlowingMushroom, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
