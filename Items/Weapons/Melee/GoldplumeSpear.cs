using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GoldplumeSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Goldplume Spear");
            Tooltip.SetDefault("Shoots falling feathers");
            SacrificeTotal = 1;
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.damage = 48;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 23;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 54;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<GoldplumeSpearProjectile>();
            Item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(10).
                AddIngredient(ItemID.SunplateBlock, 4).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}
