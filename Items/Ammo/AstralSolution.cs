using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Ammo
{
    public class AstralSolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Solution");
            Tooltip.SetDefault("Used by the Clentaminator\n" +
            "Spreads the Astral Infection");
        }

        public override void SetDefaults()
        {
            item.ammo = AmmoID.Solution;
            item.shoot = ModContent.ProjectileType<AstralSpray>() - ProjectileID.PureSpray;
            item.width = 10;
            item.height = 12;
            item.value = Item.buyPrice(0, 0, 5, 0);
            item.rare = ItemRarityID.Orange;
            item.maxStack = 999;
            item.consumable = true;
            return;
        }

        public override bool ConsumeAmmo(Player player)
        {
            return !(player.itemAnimation < player.ActiveItem().useAnimation - 3);
        }
    }
}
