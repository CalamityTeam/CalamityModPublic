using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class BelchingSaxophone : ModItem
    {
        public const int BaseDamage = 32;
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belching Saxophone");
            Tooltip.SetDefault("Doot\n" +
            "Fires an array of dirty reeds, music notes, and sulphuric bubbles");
        }

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
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AcidicReed>();
            //If a saxophone actually fired reeds, I'd be concerned.
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            counter++;

            if (Main.rand.NextBool(2))
            {
                Vector2 speed = new Vector2(speedX, speedY).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-15, 16)));
                speed.Normalize();
                speed *= 15f;
                speed.Y -= Math.Abs(speed.X) * 0.2f;
                Projectile.NewProjectile(position, speed, ModContent.ProjectileType<AcidicSaxBubble>(), damage, knockBack, player.whoAmI);
            }

            speedX += Main.rand.Next(-40, 41) * 0.05f;
            speedY += Main.rand.Next(-40, 41) * 0.05f;
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, counter % 2 == 0 ? 1f : 0f);

            if (Main.rand.NextBool(2))
            {
                int noteProj = Utils.SelectRandom(Main.rand, new int[]
                {
                    ProjectileID.QuarterNote,
                    ProjectileID.EighthNote,
                    ProjectileID.TiedEighthNote
                });
                int note = Projectile.NewProjectile(position.X, position.Y, speedX * 0.75f, speedY * 0.75f, noteProj, (int)(damage * 0.75), knockBack, player.whoAmI);
                if (note.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[note].Calamity().forceMagic = true; //why are these notes also internally ranged
                    Main.projectile[note].usesLocalNPCImmunity = true;
                    Main.projectile[note].localNPCHitCooldown = 10;
                }
            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(0, 18);
    }
}
