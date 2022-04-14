using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Seadragon : ModItem
    {
        private int shotType = 1;
        private bool rocket = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seadragon");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires streams of water every other shot\n" +
                "Fires a homing rocket every 18 shots, which explodes into fire shards on death");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 90;
            Item.height = 38;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;

            if (shotType > 17)
            {
                shotType = 1;
                rocket = true;
            }

            if (!rocket)
            {
                if (shotType % 2 == 1)
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
                else
                    Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ArcherfishShot>(), damage, knockback, player.whoAmI, 0f, 0f);

                shotType++;
            }
            else
            {
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<SeaDragonRocket>(), (int)(damage * 5), knockback, player.whoAmI, 0f, 0f);
                rocket = false;
            }

            return false;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            if (shotType % 2 == 0)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<Megalodon>()).AddIngredient(ModContent.ItemType<Phantoplasm>(), 9).AddIngredient(ModContent.ItemType<ArmoredShell>(), 3).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddIngredient(ModContent.ItemType<DepthCells>(), 15).AddIngredient(ModContent.ItemType<Lumenite>(), 15).AddIngredient(ModContent.ItemType<Tenebris>(), 5).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
