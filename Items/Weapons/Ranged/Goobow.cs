using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Goobow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goobow");
            Tooltip.SetDefault("Fires two streams of slime");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 33;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 50;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 12, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOver10 = 0.1f * MathHelper.Pi;
            int projAmt = 2;

            velocity.Normalize();
            velocity *= 20f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int i = 0; i < projAmt; i++)
            {
                float offsetAmt = i - (projAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOver10 * offsetAmt), default);
                if (!canHit)
                {
                    offset -= velocity;
                }
                int index = Projectile.NewProjectile(spawnSource, source + offset, velocity * 0.6f, ProjectileID.SlimeGun, damage / 4, 0f, player.whoAmI);
                if (index.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[index].Calamity().forceRanged = true;
                    Main.projectile[index].usesLocalNPCImmunity = true;
                    Main.projectile[index].localNPCHitCooldown = 10;
                }
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(18).
                AddIngredient(ItemID.Gel, 30).
                AddIngredient(ItemID.HellstoneBar, 5).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
