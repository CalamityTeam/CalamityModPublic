using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StaffOfNecrosteocytes : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of Necrosteocytes");
            Tooltip.SetDefault("Summons small skeletons to fight for you\n" +
                               "The skeletons leave behind bone cells as they move");
        }

        public override void SetDefaults()
        {
            Item.damage = 31;
            Item.mana = 10;
            Item.width = 52;
            Item.height = 54;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SmallSkeletonMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Point mouseTileCoords = position.ToTileCoordinates();
            if (WorldGen.SolidTile(mouseTileCoords.X, mouseTileCoords.Y))
                return false;
            if (player.altFunctionUse != 2)
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }
    }
}
