using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class UrchinSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Spear");
            Tooltip.SetDefault("Poisons enemies and fires short-range stingers");
        }

        public override void SetDefaults()
        {
            Item.width = 56;
            Item.damage = 28;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 25;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 56;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<UrchinSpearProjectile>();
            Item.shootSpeed = 4f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictideBar>(4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
