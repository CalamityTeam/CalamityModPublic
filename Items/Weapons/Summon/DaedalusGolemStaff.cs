using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DaedalusGolemStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Golem Staff");
            Tooltip.SetDefault("Summons a Daedalus Golem soldier that fires pellets and electricity\n" +
                               "Shining god of greed"); // Funny Hollow Knight reference.
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.mana = 10;
            Item.width = Item.height = 32;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item67;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DaedalusGolem>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;

            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Point mouseTileCoords = Main.MouseWorld.ToTileCoordinates();
            if (!CalamityUtils.ParanoidTileRetrieval(mouseTileCoords.X, mouseTileCoords.Y).IsTileSolidGround())
            {
                int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.UnitY * 4f, type, damage, knockback, player.whoAmI, 0f, 0f);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = Item.damage;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
