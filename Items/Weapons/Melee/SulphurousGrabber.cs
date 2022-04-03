using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SulphurousGrabber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphurous Grabber");
            Tooltip.SetDefault("Occasionally releases a ring of colored bubbles\n" +
            "The yoyo powers up after touching a green bubble\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 36;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 82;
            Item.knockBack = 3.5f;
            Item.useTime = 25;
            Item.useAnimation = 25;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<SulphurousGrabberYoyo>();
            Item.shootSpeed = 12f;

            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 36);
        }
    }
}
