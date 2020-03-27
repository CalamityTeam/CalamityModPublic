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
			"The yoyo receives powers up after touching a green bubble");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 50;
            item.height = 36;
            item.melee = true;
            item.damage = 45;
            item.knockBack = 3.5f;
            item.useTime = 25;
            item.useAnimation = 25;

            item.useStyle = 5;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<SulphurousGrabberYoyo>();
            item.shootSpeed = 12f;

            item.rare = 5;
            item.value = Item.buyPrice(gold: 36);
        }
    }
}
