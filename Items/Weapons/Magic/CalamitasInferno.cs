using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CalamitasInferno : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lashes of Chaos");
            Tooltip.SetDefault("Watch the world burn...");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 28;
            Item.height = 30;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneHellfireballFriendly>();
            Item.shootSpeed = 9f;
        }
    }
}
