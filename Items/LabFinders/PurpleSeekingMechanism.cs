using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class PurpleSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purple Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab within the sky");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<PurpleLabSeeker>();
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 15;
            Item.Calamity().UsesCharge = true;
            Item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && CalamityWorld.PlanetoidLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LabSeekingMechanism>().
                AddIngredient(ItemID.Cloud, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
