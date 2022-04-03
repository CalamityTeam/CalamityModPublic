using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SlitheringEels : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slithering Eels");
            Tooltip.SetDefault("Casts a magical acid eel that releases acid drops as it moves");
        }

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 11;
            Item.width = 34;
            Item.height = 40;
            Item.useAnimation = Item.useTime = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.NPCHit13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlitheringEelProjectile>();
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedByRandom(0.325f), type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
