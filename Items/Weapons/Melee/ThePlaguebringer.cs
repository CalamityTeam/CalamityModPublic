using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ThePlaguebringer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pandemic");
            Tooltip.SetDefault("Fires plague seekers when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 32;
            item.melee = true;
            item.damage = 66;
            item.knockBack = 2.5f;
            item.useTime = 22;
            item.useAnimation = 22;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<PandemicYoyo>();
            item.shootSpeed = 14f;

            item.rare = ItemRarityID.Yellow;
            item.value = Item.buyPrice(gold: 80);
        }
    }
}
