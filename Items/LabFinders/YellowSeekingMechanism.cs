using CalamityMod.Projectiles.Typeless;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LabFinders
{
    public class YellowSeekingMechanism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yellow Seeking Mechanism");
            Tooltip.SetDefault("Moves swiftly towards a lab within the frozen caverns");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<YellowLabSeeker>();
            Item.Calamity().MaxCharge = 100;
            Item.Calamity().ChargePerUse = 15;
            Item.Calamity().UsesCharge = true;
            Item.Calamity().customRarity = CalamityRarity.DraedonRust;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0 && CalamityWorld.IceLabCenter != Vector2.Zero;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LabSeekingMechanism>().
                AddIngredient(ItemID.IceBlock, 50).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
