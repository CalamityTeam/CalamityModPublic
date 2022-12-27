using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Poseidon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon");
            Tooltip.SetDefault("Casts a poseidon typhoon");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.UseSound = SoundID.Item84;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PoseidonTyphoon>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int typhoon = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            Main.projectile[typhoon].penetrate = Main.rand.Next(4,11);
            return false;
        }
    }
}
