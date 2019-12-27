using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheObliterator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Obliterator");
            Tooltip.SetDefault("Ruins nearby enemies with death lasers\nAn exceptionally agile yoyo\n");
            ItemID.Sets.Yoyo[item.type] = true;
            ItemID.Sets.GamepadExtraRange[item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 40;
            item.melee = true;
            item.damage = 370;
            item.knockBack = 7.5f;
            item.useTime = 20;
            item.useAnimation = 20;
            item.autoReuse = true;

            item.useStyle = 5;
            item.UseSound = SoundID.Item1;
            item.channel = true;
            item.noUseGraphic = true;
            item.noMelee = true;

            item.shoot = ModContent.ProjectileType<TheObliteratorYoyo>();
            item.shootSpeed = 16f;

            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.value = Item.buyPrice(platinum: 1, gold: 40);
        }
    }
}
