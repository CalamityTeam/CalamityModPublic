using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Aftershock : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.damage = 65;
            Item.DamageType = DamageClass.Melee;
            Item.width = 54;
            Item.height = 58;
            Item.scale = 1.75f;
            Item.useTime = 28;
            Item.useAnimation = 28;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shootSpeed = 12f;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            float rockSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX - Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY - Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = rockSpeed;
            }
            else
            {
                mouseDistance = rockSpeed / mouseDistance;
            }

            realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
            realPlayerPos.Y -= (float)100;
            mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (mouseYDist < 0f)
            {
                mouseYDist *= -1f;
            }
            if (mouseYDist < 20f)
            {
                mouseYDist = 20f;
            }
            mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            mouseDistance = rockSpeed / mouseDistance;
            mouseXDist *= mouseDistance;
            mouseYDist *= mouseDistance;
            float speedX4 = mouseXDist;
            float speedY5 = mouseYDist + (float)Main.rand.Next(-10, 11) * 0.02f;
            int rockDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<AftershockRock>(), rockDamage, hit.Knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 300);
            float rockSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX - Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY - Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = rockSpeed;
            }
            else
            {
                mouseDistance = rockSpeed / mouseDistance;
            }

            realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
            realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
            realPlayerPos.Y -= (float)100;
            mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (mouseYDist < 0f)
            {
                mouseYDist *= -1f;
            }
            if (mouseYDist < 20f)
            {
                mouseYDist = 20f;
            }
            mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            mouseDistance = rockSpeed / mouseDistance;
            mouseXDist *= mouseDistance;
            mouseYDist *= mouseDistance;
            float speedX4 = mouseXDist;
            float speedY5 = mouseYDist + (float)Main.rand.Next(-10, 11) * 0.02f;
            int rockDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
            Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<AftershockRock>(), rockDamage, Item.knockBack, player.whoAmI, 0f, (float)Main.rand.Next(10));
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 32);
            }
        }
    }
}
