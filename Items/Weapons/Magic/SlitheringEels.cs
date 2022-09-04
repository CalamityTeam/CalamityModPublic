using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
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
            Item.value = CalamityGlobalItem.Rarity6BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.NPCHit13;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SlitheringEelProjectile>();
            Item.shootSpeed = 14f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(0.325f), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
