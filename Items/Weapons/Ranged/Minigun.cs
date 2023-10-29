using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Minigun : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle AuricFire = new("CalamityMod/Sounds/Item/MinigunAuricFire") { Volume = 0.7f };
        public static readonly SoundStyle RevSound = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLV2") { Volume = 0.7f };
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 321;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 92;
            Item.height = 44;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<MinigunHoldout>();
            Item.shootSpeed = 2f;
            Item.useAmmo = AmmoID.Bullet;
            Item.rare = ModContent.RarityType<Violet>();
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.NextFloat() > 0.95f && player.ownedProjectileCounts[Item.shoot] > 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<MinigunHoldout>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ChainGun).
                AddIngredient<ClockGatlignum>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
