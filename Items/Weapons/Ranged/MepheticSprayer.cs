using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class MepheticSprayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blight Spewer");
        }

        public override void SetDefaults()
        {
            item.damage = 110;
            item.ranged = true;
            item.width = 76;
            item.height = 36;
            item.useTime = 10;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CorossiveFlames>();
            item.shootSpeed = 7.5f;
            item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
