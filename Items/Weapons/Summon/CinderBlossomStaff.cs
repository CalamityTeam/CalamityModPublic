using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CinderBlossomStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cinder Blossom Staff");
            Tooltip.SetDefault("Summons a really hot flower over your head\n" +
                "There can only be one flower");
        }

        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 46;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item34;
            Item.shoot = ModContent.ProjectileType<CinderBlossom>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Fireblossom, 5).AddIngredient(ItemID.LavaBucket, 2).AddIngredient(ItemID.HellstoneBar, 10).AddTile(TileID.Anvils).Register();
        }
    }
}
