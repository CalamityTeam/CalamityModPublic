using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ForbiddenSun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forbidden Sun");
            Tooltip.SetDefault("Casts a fire orb that emits a gigantic explosion on death");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 33;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ForbiddenSunProjectile>();
            Item.shootSpeed = 9f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<ScoriaBar>(6).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
