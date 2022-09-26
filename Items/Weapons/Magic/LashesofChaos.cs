using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("CalamitasInferno")]
    public class LashesofChaos : ModItem
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
            Item.useTime = 38;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneHellfireballFriendly>();
            Item.shootSpeed = 3f;
        }
    }
}
