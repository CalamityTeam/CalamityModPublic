using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GloriousEnd : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        // Scales with difficulty (Expert: 60-80)
        public static int PlayerExplosionDmgMin = 30;
        public static int PlayerExplosionDmgMax = 40;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 58;
            Item.damage = 60;
            Item.knockBack = 10f;
            Item.useAnimation = Item.useTime = 30;
            Item.mana = 60;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<MeteorStar>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item9;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;
        }

        public override bool? CanAutoReuseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<GiantIbanRobotOfDoom>()] > 0;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;
    }
}
