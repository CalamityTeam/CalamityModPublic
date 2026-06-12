using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SulphuricAcidCannon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<SulphuricPoisoning>()];
        }
        public override void SetDefaults()
        {
            Item.width = 90;
            Item.height = 30;
            Item.damage = 550;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.UseSound = SoundID.Item73;
            Item.useAmmo = AmmoID.Rocket;
            Item.shoot = ModContent.ProjectileType<AcidRocket>();
            Item.shootSpeed = 16f;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override Vector2? HoldoutOffset() => Vector2.UnitX * -15f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip + add some inaccuracy
            Vector2 newPos = position + new Vector2(78f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(10f));

            //Find which rocket type the player will be using.
            player.PickAmmo(player.HeldItem, out _, out _, out _, out _, out int rocketType);
            Projectile.NewProjectile(source, newPos, newVel, Item.shoot, damage, knockback, player.whoAmI,rocketType);
            return false;
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            float pullback = 7f;

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.4f)
                pullback -= (2.75f) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2);

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * pullback;
            Vector2 itemSize = new Vector2(52, 28);
            Vector2 itemOrigin = new Vector2(-24, 4);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
