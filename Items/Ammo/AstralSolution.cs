using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            Item.ammo = AmmoID.Solution;
            Item.shoot = ModContent.ProjectileType<AstralSpray>() - ProjectileID.PureSpray;
            Item.width = 10;
            Item.height = 12;
            Item.value = Item.buyPrice(0, 0, 5, 0);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 999;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            Item.consumable = true;
            return;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            return !(player.itemAnimation < player.ActiveItem().useAnimation - 3);
        }
    }
}
