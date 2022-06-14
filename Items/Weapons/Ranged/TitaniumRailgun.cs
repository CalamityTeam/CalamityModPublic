using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class TitaniumRailgun : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Railgun");
            Tooltip.SetDefault("Hold down to charge up a decimating titanium laser.\n" +
                "Longer hold time increases power, leading to more damage, size, and knockback.\n" +
                "Power is capped when the cannon's sights have fully converged.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 275;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 62;
            Item.height = 32;
            Item.useTime = 26;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 5f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item77 with { Volume = SoundID.Item77.Volume * 0.7f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<TitaniumRailgunScope>();
            Item.shootSpeed = 16f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 4);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: TitaniumRailgunScope.BaseMaxCharge * player.GetWeaponAttackSpeed(player.HeldItem));
            return false;
        }

        public override void UseItemFrame(Player player)
        {
            // Thank you Mr. IbanPlay (CoralSprout.cs)
            // Calculate the dirction in which the players arms should be pointing at.
            float armPointingDirection = player.itemRotation;
            if (player.direction < 0)
                armPointingDirection += MathHelper.Pi;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armPointingDirection - MathHelper.PiOver2);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.TitaniumBar, 10).
                AddIngredient(ItemID.CrystalShard, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
