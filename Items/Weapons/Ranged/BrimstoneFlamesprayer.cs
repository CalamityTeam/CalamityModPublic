using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BrimstoneFlamesprayer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Havoc's Breath");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 59;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 18;
            Item.useTime = 9;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.UseSound = SoundID.Item34;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BrimstoneFireFriendly>();
            Item.shootSpeed = 8.5f;
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
