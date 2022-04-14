using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class DragoonDrizzlefish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragoon Drizzlefish");
            Tooltip.SetDefault(@"Fires an inaccurate spread of fireballs
The brimstone sac appears to contain fuel
Revenge is a dish best served flaming hot");
            Item.staff[Item.type] = true; //so it doesn't look weird af when holding it
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DrizzlefishFireball>();
            Item.shootSpeed = 11f;
        }

        public override Vector2? HoldoutOrigin() //so it looks normal when holding
        {
            return new Vector2(10, 10);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(10f));
            int shotType = ModContent.ProjectileType<DrizzlefishFireball>();
            if (Main.rand.NextBool(2))
            {
                shotType = ModContent.ProjectileType<DrizzlefishFire>();
            }
            else
            {
                shotType = ModContent.ProjectileType<DrizzlefishFireball>();
            }
            Projectile.NewProjectile(source, position, velocity, shotType, damage, knockback, player.whoAmI, 0f, Main.rand.Next(2));
            return false;
        }
    }
}
