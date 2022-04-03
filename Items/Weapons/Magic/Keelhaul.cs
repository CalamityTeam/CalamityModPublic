using CalamityMod.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Keelhaul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keelhaul");
            Tooltip.SetDefault("Summons a geyser upon hitting an enemy\n" +
                "Crumple 'em like paper");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 55;
            Item.mana = 50;
            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;

            Item.shoot = ModContent.ProjectileType<KeelhaulBubble>();
            Item.shootSpeed = 15f;
        }
    }
}
