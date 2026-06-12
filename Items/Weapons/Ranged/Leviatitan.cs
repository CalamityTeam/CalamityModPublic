using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Leviatitan : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        // Every sixth shot fires homing Aberrations, then resets the counter to 2.
        // This is intentional so you don't get the Aberrations instantly when you start firing and have to play around it.
        private int shotCounter = 1;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<RiptideDebuff>(), ModContent.BuffType<ArmorCrunch>()];
        }

        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 28;
            Item.damage = 89;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 18; //Try not to change this if you can help it.
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") { Pitch = 0.65f, Volume = 0.4f };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AquaBlast>();
            Item.shootSpeed = 13f;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            Item.UseSound = player.altFunctionUse == 2 ? SoundID.NPCHit56 : new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") { Pitch = 0.65f, Volume = 0.3f };
            return base.CanUseItem(player);
        }
        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? (1f / 3f) : 1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reposition to the gun's tip. Calculated separately due to the two different shot styles.
            Vector2 boulderPos = position + new Vector2(74f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -2f : -8f)).RotatedBy(velocity.ToRotation());
            Vector2 shotPos = position + new Vector2(74f, player.direction * (Math.Abs(velocity.SafeNormalize(Vector2.Zero).X) < 0.02f ? -6f : 3f)).RotatedBy(velocity.ToRotation());

            if (player.altFunctionUse == 2)
            {
                int index = Projectile.NewProjectile(source, boulderPos, velocity * 0.7f, ModContent.ProjectileType<LeviatitanMeteor>(), (int)(damage * 2f), knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, shotPos, velocity, Item.shoot, damage, knockback, player.whoAmI);
            }
            if (shotCounter == 7)
            {
                SoundEngine.PlaySound(SoundID.Zombie38 with { Volume = SoundID.Zombie38.Volume * 0.5f });
                for (int i = 0; i <= 1; i++)
                {
                    float projSpeed = Item.shootSpeed;
                    Projectile.NewProjectile(source, position + Main.rand.NextVector2Circular(150, 150), velocity * projSpeed, ModContent.ProjectileType<LeviatitanAberration>(), (int)(damage * 1.3), knockback, player.whoAmI);
                }      
            }
            shotCounter++;
            if (shotCounter > 7)
                shotCounter = 2;
            return false;
        }

        //Thanks to Xyk's incredible recoil code, we give both left click and right click different recoils without having to use a holdout.
        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            float pullback = 7f;

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.1f)
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
            if (animProgress < 0.4f)
                rotation += (player.altFunctionUse == 2 ? -0.15f : 0) * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}

