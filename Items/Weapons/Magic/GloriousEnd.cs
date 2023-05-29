using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GloriousEnd : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Magic";
        public static int PlayerExplosionDmgMin = 50;
        public static int PlayerExplosionDmgMax = 60;
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.knockBack = 10f;
            Item.useTime = Item.useAnimation = 30;
            Item.mana = 20;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<MeteorStar>();

            Item.width = 30;
            Item.height = 58;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item9;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.Calamity().donorItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
