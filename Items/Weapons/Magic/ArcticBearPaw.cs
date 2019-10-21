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
            Tooltip.SetDefault(@"Fires spiritual claws that ignore walls and confuse enemies
'The savage mauling that fits in your pocket'");
        }
        public override void SetDefaults()
        {
            item.damage = 70;
            item.magic = true;
            item.mana = 18;
            item.width = 34;
            item.height = 22;
            item.useTime = 28;
            item.useAnimation = 28;
            item.useStyle = 5;
            item.useTurn = false;
            item.noMelee = true;
            item.knockBack = 10f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item8;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ArcticBearPawProj>();
            item.shootSpeed = 27f;
        }
    }
}
