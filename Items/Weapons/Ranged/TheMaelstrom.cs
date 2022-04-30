using Terraria.DataStructures;
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
    public class TheMaelstrom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Maelstrom");
            Tooltip.SetDefault("Fires charged Reaper Sharks that explode into water");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 397;
            Item.width = 20;
            Item.height = 12;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.rare = ItemRarityID.Purple;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MaelstromHoldout>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
            Item.Calamity().donorItem = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<MaelstromHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TheStorm>().
                AddIngredient<ReaperTooth>(3).
                AddIngredient<DivineGeode>(20).
                AddIngredient<Voidstone>(50).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
