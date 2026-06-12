using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SeethingDischarge : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public int DartTimer = 0;
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<BrimstoneFlames>()];
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 40;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useAnimation = Item.useTime = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.75f;
            Item.UseSound = BrimstoneElemental.ShellFireSound;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SeethingDischargeBrimstoneBarrage>();
            Item.shootSpeed = 10f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            DartTimer++;

            // Spawn hellblasts
            float diameter = 50f;
            Vector2 projVelocity = velocity.SafeNormalize(Vector2.UnitY);
            projVelocity *= diameter;

            int totalProjectiles = 5;
            float offsetAngle = MathHelper.Pi * 0.2f;

            for (int j = 0; j < totalProjectiles; j++)
            {
                float radians = j - (totalProjectiles - 1f) / 2f;
                Vector2 offset = projVelocity.RotatedBy(offsetAngle * radians);
                Projectile.NewProjectile(source, player.Center + offset, velocity, ModContent.ProjectileType<SeethingDischargeBrimstoneHellblast>(), damage, knockback, player.whoAmI);
            }

            // Every 3rd attack, spawn a ring of homing darts
            if (DartTimer == 3)
            {
                DartTimer = 0;

                totalProjectiles = 8;
                Vector2 initialOffset = Vector2.UnitX.RotatedBy(Main.rand.NextFloat(0f, MathHelper.PiOver4));
                for (int k = 0; k < totalProjectiles; k++)
                {
                    velocity = initialOffset.RotatedBy(MathHelper.TwoPi * (k / (float)totalProjectiles)) * Item.shootSpeed;
                    Projectile.NewProjectile(source, player.Center, velocity, type, damage, 0f, player.whoAmI, 0f, 1f);
                }
            }

            return false;
        }
    }
}
