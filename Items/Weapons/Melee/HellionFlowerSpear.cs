using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee.Spears;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class HellionFlowerSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellion Flower Spear");
            Tooltip.SetDefault("Shoots a flower spear tip\n" +
                "Summons petals from the sky on critical hits");
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.damage = 135;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 64;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<HellionFlowerSpearProjectile>();
            Item.shootSpeed = 8f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<DraedonBar>(), 12).AddTile(TileID.MythrilAnvil).Register();
        }
    }
}
