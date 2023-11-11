using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Earth : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 92;
            Item.height = 104;
            Item.scale = 1.5f;
            Item.damage = 170;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 16;
            Item.useTurn = true;
            Item.knockBack = 9.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;

            Item.value = CalamityGlobalItem.Rarity16BuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            float projSpeed = 25f;
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
                mouseDistance = projSpeed;
            }
            else
            {
                mouseDistance = projSpeed / mouseDistance;
            }

            for (int i = 0; i < 3; i++)
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
                float speedX4 = mouseXDist;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-180, 181) * 0.02f;

                int projDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<EarthProj>(), projDamage, hit.Knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }

            if (target.Calamity().miscDefenseLoss < target.defense)
                target.Calamity().miscDefenseLoss += 1;

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int heal = Main.rand.Next(1, 69);
            player.statLife += heal;
            player.HealEffect(heal);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            float projSpeed = 25f;
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
                mouseDistance = projSpeed;
            }
            else
            {
                mouseDistance = projSpeed / mouseDistance;
            }

            for (int i = 0; i < 3; i++)
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
                float speedX4 = mouseXDist;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-180, 181) * 0.02f;
                int earthDamage = player.CalcIntDamage<MeleeDamageClass>(Item.damage);
                Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<EarthProj>(), earthDamage, Item.knockBack, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }

            if (player.moonLeech)
                return;

            int heal = Main.rand.Next(1, 69);
            player.statLife += heal;
            player.HealEffect(heal);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GrandGuardian>().
                AddIngredient<GalactusBlade>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
