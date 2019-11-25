using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SolarFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Flare");
            Tooltip.SetDefault("Emits large holy explosions on hit");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 60;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SolarFlareYoyo>();
            item.Calamity().postMoonLordRarity = 12;
            ItemID.Sets.Yoyo[item.type] = true;
        }
    }
}
