using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArcticBearPaw : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arctic Bear Paw");
            Tooltip.SetDefault(@"The savage mauling that fits in your pocket
Fires spiritual claws that ignore walls");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.damage = 90;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 18;
            Item.width = 28;
            Item.height = 24;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTurn = false;
            Item.noMelee = true;
            Item.knockBack = 10f;
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.LightPurple;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ArcticBearPawProj>();
            Item.shootSpeed = 27f;
        }
    }
}
