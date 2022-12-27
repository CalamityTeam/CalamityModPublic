using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DreadmineStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dreadmine Staff");
            Tooltip.SetDefault("Summons a dreadmine turret to fight for you");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.mana = 10;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 4.5f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item113;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DreadmineTurret>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Voidstone>(30).
                AddIngredient<DepthCells>(30).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
