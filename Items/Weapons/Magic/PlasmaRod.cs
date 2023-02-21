using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class PlasmaRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Rod");
            Tooltip.SetDefault("Casts a low-damage plasma bolt\n" +
                "Shooting a tile will cause several bolts with increased damage to fire\n" +
                "Shooting an enemy will inflict shadowflame");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PlasmaRay>();
            Item.shootSpeed = 11f;
        }
    }
}
