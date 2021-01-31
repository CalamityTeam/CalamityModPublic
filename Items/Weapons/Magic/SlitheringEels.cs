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
            item.damage = 80;
            item.magic = true;
            item.mana = 11;
            item.width = 34;
            item.height = 40;
            item.useAnimation = item.useTime = 40;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.NPCHit13;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SlitheringEelProjectile>();
            item.shootSpeed = 14f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY).RotatedByRandom(0.325f), type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
