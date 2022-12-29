using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class GreenSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Green Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab within the jungle");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<GreenLabSeeker>();
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 10;
            Item.Calamity().UsesCharge = true;
            Item.rare = ModContent.RarityType<DarkOrange>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && CalamityWorld.JungleLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LabSeekingMechanism>().
                AddIngredient(ItemID.Vine, 3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
