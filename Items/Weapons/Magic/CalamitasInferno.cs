using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CalamitasInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lashes of Chaos");
            Tooltip.SetDefault("Watch the world burn...");
        }

        public override void SetDefaults()
        {
            item.damage = 65;
            item.magic = true;
            item.mana = 20;
            item.width = 28;
            item.height = 30;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BrimstoneHellfireballFriendly>();
            item.shootSpeed = 9f;
        }
    }
}
