using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PlasmaRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Rod");
            Tooltip.SetDefault("Casts a low-damage plasma bolt\n" +
                "Shooting a tile will cause several bolts with increased damage to fire\n" +
                "Shooting an enemy will cause several debuffs for a short time");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 8;
            item.magic = true;
            item.mana = 10;
            item.width = 40;
            item.height = 40;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2.5f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PlasmaRay>();
            item.shootSpeed = 11f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(10, 10);
        }
    }
}
