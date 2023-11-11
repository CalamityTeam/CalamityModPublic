using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class OverloadedBlaster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 42;
            Item.height = 34;
            Item.useAnimation = Item.useTime = 22;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item9;
            Item.autoReuse = true;
            Item.shootSpeed = 5f;
            Item.shoot = ModContent.ProjectileType<SlimeBolt>();
            Item.useAmmo = AmmoID.Gel;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-4, -5);

        public override bool CanConsumeAmmo(Item ammo, Player player) => Main.rand.Next(100) >= 25;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newPos = position + velocity * 6f;
            for (int i = 0; i < 3; i++)
            {
                Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.8f, 1.2f);
                Projectile.NewProjectile(source, newPos, newVel, type, damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
}
