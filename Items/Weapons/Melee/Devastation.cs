using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Devastation : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 11));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 72;
            Item.damage = 132;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 72;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<GalaxyBlast>();
            Item.shootSpeed = 16f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (Main.rand.Next(6))
            {
                case 1:
                    type = ModContent.ProjectileType<GalaxyBlast>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<GalaxyBlastType2>();
                    break;
                case 3:
                    type = ModContent.ProjectileType<GalaxyBlastType3>();
                    break;
                default:
                    break;
            }
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, Main.myPlayer);
            float projSpeed = Item.shootSpeed;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt((double)(mouseXDist * mouseXDist + mouseYDist * mouseYDist));
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = (float)player.direction;
                mouseYDist = 0f;
                mouseDistance = projSpeed;
            }
            else
            {
                mouseDistance = projSpeed / mouseDistance;
            }

            for (int i = 0; i < 4; i++)
            {
                realPlayerPos = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -(float)player.direction) + ((float)Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y - 600f);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
                realPlayerPos.Y -= (float)(100 * i);
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
                mouseDistance = projSpeed / mouseDistance;
                mouseXDist *= mouseDistance;
                mouseYDist *= mouseDistance;
                float speedX4 = mouseXDist + (float)Main.rand.Next(-40, 41) * 0.02f;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-40, 41) * 0.02f;
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<GalaxyBlast>(), damage / 2, knockback, player.whoAmI, 0f, (float)Main.rand.Next(5));
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<GalaxyBlastType2>(), damage / 2, knockback, player.whoAmI, 0f, (float)Main.rand.Next(3));
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<GalaxyBlastType3>(), damage / 2, knockback, player.whoAmI, 0f, (float)Main.rand.Next(1));
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 73);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Ichor, 60);
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CatastropheClaymore>().
                AddIngredient(ItemID.LunarBar, 5).
                AddIngredient<AstralBar>(10).
                AddIngredient(ItemID.MeteoriteBar, 10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
