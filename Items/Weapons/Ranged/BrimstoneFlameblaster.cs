using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BrimstoneFlameblaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Flameblaster");
            Tooltip.SetDefault("Fires bouncing brimstone fireballs");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 64;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneBallFriendly>();
            Item.shootSpeed = 10f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
