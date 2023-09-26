using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BelchingSaxophone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public const int BaseDamage = 32;
        private int counter = 0;

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 46;
            Item.height = 22;
            Item.useTime = 12;
            Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AcidicReed>();
            //If a saxophone actually fired reeds, I'd be concerned.
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            counter++;

            if (Main.rand.NextBool())
            {
                Vector2 speed = velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                speed.Normalize();
                speed *= 15f;
                speed.Y -= Math.Abs(speed.X) * 0.2f;
                Projectile.NewProjectile(source, position, speed, ModContent.ProjectileType<AcidicSaxBubble>(), damage, knockback, player.whoAmI);
            }

            velocity.X += Main.rand.Next(-40, 41) * 0.05f;
            velocity.Y += Main.rand.Next(-40, 41) * 0.05f;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, counter % 2 == 0 ? 1f : 0f);

            if (Main.rand.NextBool())
            {
                int noteProj = Utils.SelectRandom(Main.rand, new int[]
                {
                    ProjectileID.QuarterNote,
                    ProjectileID.EighthNote,
                    ProjectileID.TiedEighthNote
                });
                int note = Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 0.75f, velocity.Y * 0.75f, noteProj, (int)(damage * 0.75), knockback, player.whoAmI);
                if (note.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[note].DamageType = DamageClass.Magic; //why are these notes also internally ranged
                    Main.projectile[note].usesLocalNPCImmunity = true;
                    Main.projectile[note].localNPCHitCooldown = 10;
                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 18);
    }
}
