using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class HadalUrn : ModItem
    {
        public static readonly SoundStyle ShootSound = new("CalamityMod/Sounds/Item/HadalUrnOpen");
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadal Urn");
            Tooltip.SetDefault("Fires a random assortment of ocean creatures\n"+ "'There's no telling how long it was down there... or what lurks inside'");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 61;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 58;
            Item.height = 38;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.75f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.Calamity().donorItem = true;
            Item.UseSound = ShootSound;
            Item.autoReuse = true;
            Item.shootSpeed = 8f;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<HadalUrnHoldout>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SoundEngine.PlaySound(SoundID.Item103 with { Volume = SoundID.Item103.Volume }, player.position);
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<HadalUrnHoldout>(), damage, knockback, player.whoAmI, 12);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlackAnurian>().
                AddIngredient<SmoothVoidstone>(20).
                AddIngredient<Lumenyl>(5).
                AddIngredient<DepthCells>(15).
                AddIngredient(ItemID.Bone, 10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
