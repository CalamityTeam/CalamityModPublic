using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("GalacticaBlade")]
    public class GalactusBlade : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override void SetDefaults()
        {
            Item.width = 60;
            Item.damage = 84;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 17;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.height = 58;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.shoot = ModContent.ProjectileType<GalacticaComet>();
            Item.shootSpeed = 23f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float cometSpeed = Item.shootSpeed;
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
                mouseDistance = cometSpeed;
            }
            else
            {
                mouseDistance = cometSpeed / mouseDistance;
            }

            for (int i = 0; i < 5; i++)
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
                mouseDistance = cometSpeed / mouseDistance;
                mouseXDist *= mouseDistance;
                mouseYDist *= mouseDistance;
                float speedX4 = mouseXDist + (float)Main.rand.Next(-100, 101) * 0.02f;
                float speedY5 = mouseYDist + (float)Main.rand.Next(-100, 101) * 0.02f;
                int projectile = Projectile.NewProjectile(source, realPlayerPos.X, realPlayerPos.Y, speedX4, speedY5, ModContent.ProjectileType<GalacticaComet>(), damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(10));
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, Main.rand.NextBool() ? 164 : 229);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.StarWrath).
                AddIngredient<DivineGeode>(10).
                AddIngredient<GalacticaSingularity>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
