using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class GodSlayerSlug : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Slug");
            Tooltip.SetDefault("Heavy ammunition with unlimited piercing that tears through spacetime\n" +
                "After a slug lands a hit, if it strikes a wall or runs out of targets to pierce,\n" +
                "it warps backwards through space and supercharges, exploding on impact");
        }

        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 28);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.shoot = ModContent.ProjectileType<GodSlayerSlugProj>();
            Item.shootSpeed = 6f;
            Item.ammo = ItemID.MusketBall;
        }

        public override void AddRecipes()
        {
            CreateRecipe(999).
                AddIngredient(ItemID.EmptyBullet, 999).
                AddIngredient<CosmiliteBar>().
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
