using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMicrowave : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            Tooltip.SetDefault("Fries nearby enemies with radiation\n" +
            "A very agile yoyo\n" +
            "Cooking, Astral Infection style");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 34;
            item.melee = true;
            item.damage = 65;
            item.knockBack = 3f;
            item.useTime = 22;
            item.useAnimation = 22;
            item.autoReuse = true;

            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<MicrowaveYoyo>();
            item.shootSpeed = 14f;

            item.rare = ItemRarityID.Cyan;
            item.value = Item.buyPrice(gold: 95);
        }
    }
}
