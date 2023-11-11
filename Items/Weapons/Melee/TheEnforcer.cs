using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheEnforcer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 100;
            Item.height = 100;
            Item.scale = 1.5f;
            Item.damage = 890;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TheEnforcerGlow").Value);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var source = player.GetSource_ItemUse(Item);
            SoundEngine.PlaySound(SoundID.Item73, player.Center);
            int j = Main.myPlayer;
            float flameSpeed = 3f;
            player.itemTime = Item.useTime;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = Main.mouseX + Main.screenPosition.X + realPlayerPos.X;
            float mouseYDist = Main.mouseY + Main.screenPosition.Y + realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = player.direction;
                mouseYDist = 0f;
                mouseDistance = flameSpeed;
            }
            else
            {
                mouseDistance = flameSpeed / mouseDistance;
            }

            int essenceDamage = player.CalcIntDamage<MeleeDamageClass>(0.25f * Item.damage);
            for (int i = 0; i < 5; i++)
            {
                realPlayerPos = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(401) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + Main.rand.Next(-400, 401);
                realPlayerPos.Y -= 100 * i;
                mouseXDist = Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                mouseYDist = Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (mouseYDist < 0f)
                {
                    mouseYDist *= -1f;
                }
                if (mouseYDist < 20f)
                {
                    mouseYDist = 20f;
                }
                mouseDistance = (float)Math.Sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
                mouseDistance = flameSpeed / mouseDistance;
                Projectile.NewProjectile(source, realPlayerPos, Vector2.Zero, ModContent.ProjectileType<EssenceFlame2>(), essenceDamage, 0f, i, 0f, Main.rand.Next(3));
            }
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            var source = player.GetSource_ItemUse(Item);
            SoundEngine.PlaySound(SoundID.Item73, player.Center);
            int j = Main.myPlayer;
            float flameSpeed = 3f;
            player.itemTime = Item.useTime;
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = Main.mouseX + Main.screenPosition.X + realPlayerPos.X;
            float mouseYDist = Main.mouseY + Main.screenPosition.Y + realPlayerPos.Y;
            if (player.gravDir == -1f)
            {
                mouseYDist = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + realPlayerPos.Y;
            }
            float mouseDistance = (float)Math.Sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
            if ((float.IsNaN(mouseXDist) && float.IsNaN(mouseYDist)) || (mouseXDist == 0f && mouseYDist == 0f))
            {
                mouseXDist = player.direction;
                mouseYDist = 0f;
                mouseDistance = flameSpeed;
            }
            else
            {
                mouseDistance = flameSpeed / mouseDistance;
            }

            int essenceDamage = player.CalcIntDamage<MeleeDamageClass>(0.25f * Item.damage);
            for (int i = 0; i < 5; i++)
            {
                realPlayerPos = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(401) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                realPlayerPos.X = (realPlayerPos.X + player.Center.X) / 2f + Main.rand.Next(-400, 401);
                realPlayerPos.Y -= 100 * i;
                mouseXDist = Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
                mouseYDist = Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;
                if (mouseYDist < 0f)
                {
                    mouseYDist *= -1f;
                }
                if (mouseYDist < 20f)
                {
                    mouseYDist = 20f;
                }
                mouseDistance = (float)Math.Sqrt(mouseXDist * mouseXDist + mouseYDist * mouseYDist);
                mouseDistance = flameSpeed / mouseDistance;
                Projectile.NewProjectile(source, realPlayerPos, Vector2.Zero, ModContent.ProjectileType<EssenceFlame2>(), essenceDamage, 0f, i, 0f, Main.rand.Next(3));
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmiliteBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
