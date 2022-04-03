using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TheMaelstrom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Maelstrom");
            Tooltip.SetDefault("Fires charged Reaper Sharks that explode into water");
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
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Ranged;
            Item.channel = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MaelstromHoldout>();
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).SafeNormalize(Vector2.UnitX * player.direction), ModContent.ProjectileType<MaelstromHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<TheStorm>()).AddIngredient(ModContent.ItemType<ReaperTooth>(), 3).AddIngredient(ModContent.ItemType<DivineGeode>(), 20).AddIngredient(ModContent.ItemType<Voidstone>(), 50).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
