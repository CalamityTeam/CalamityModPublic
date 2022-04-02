using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheEyeofCalamitas : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Oblivion");
            Tooltip.SetDefault("Fires brimstone lasers when enemies are near\n" +
            "A very agile yoyo");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 38;
            item.melee = true;
            item.damage = 55;
            item.knockBack = 4f;
            item.useTime = 22;
            item.useAnimation = 22;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<OblivionYoyo>();
            item.shootSpeed = 14f;

            item.rare = ItemRarityID.Lime;
            item.value = Item.buyPrice(gold: 60);
        }
    }
}
