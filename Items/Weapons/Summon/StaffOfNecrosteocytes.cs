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
            item.damage = 31;
            item.mana = 10;
            item.width = 52;
            item.height = 54;
            item.useTime = item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item44;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SmallSkeletonMinion>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Point mouseTileCoords = position.ToTileCoordinates();
            if (WorldGen.SolidTile(mouseTileCoords.X, mouseTileCoords.Y))
                return false;
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
