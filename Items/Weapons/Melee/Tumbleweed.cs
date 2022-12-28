using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Tumbleweed : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tumbleweed");
            Tooltip.SetDefault("Releases a rolling tumbleweed on hit");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 125;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 30;
            Item.height = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<TumbleweedFlail>();
            Item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Sunfury).
                AddIngredient<GrandScale>().
                AddIngredient(ItemID.SoulofMight, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
