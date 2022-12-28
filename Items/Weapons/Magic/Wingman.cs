using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Wingman : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wingman");
            Tooltip.SetDefault("Fires a concentrated laser beam");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 70;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 42;
            Item.height = 22;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item33;
            Item.autoReuse = true;
            Item.shootSpeed = 25f;
            Item.shoot = ProjectileID.LaserMachinegunLaser;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int beamAmt = 3;
            for (int index = 0; index < beamAmt; ++index)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
