using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheEnforcer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Enforcer");
            Tooltip.SetDefault("Spawns essence flames on hit");
        }

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
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/TheEnforcerGlow").Value);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            SoundEngine.PlaySound(SoundID.Item73, player.position);
            int i = Main.myPlayer;
            float num72 = 3f;
            player.itemTime = Item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = Main.mouseX + Main.screenPosition.X + vector2.X;
            float num79 = Main.mouseY + Main.screenPosition.Y + vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + vector2.Y;
            }
            float num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            int num107 = 5;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(401) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-400, 401);
                vector2.Y -= 100 * num108;
                num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
                num80 = num72 / num80;
                Projectile.NewProjectile(source, vector2, Vector2.Zero, ModContent.ProjectileType<EssenceFlame2>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f) * 0.25f), 0f, i, 0f, Main.rand.Next(3));
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            SoundEngine.PlaySound(SoundID.Item73, player.position);
            int i = Main.myPlayer;
            float num72 = 3f;
            player.itemTime = Item.useTime;
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = Main.mouseX + Main.screenPosition.X + vector2.X;
            float num79 = Main.mouseY + Main.screenPosition.Y + vector2.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + Main.screenHeight + Main.mouseY + vector2.Y;
            }
            float num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = player.direction;
                num79 = 0f;
                num80 = num72;
            }
            else
            {
                num80 = num72 / num80;
            }

            int num107 = 5;
            for (int num108 = 0; num108 < num107; num108++)
            {
                vector2 = new Vector2(player.position.X + player.width * 0.5f + (Main.rand.Next(401) * -(float)player.direction) + (Main.mouseX + Main.screenPosition.X - player.position.X), player.MountedCenter.Y);
                vector2.X = (vector2.X + player.Center.X) / 2f + Main.rand.Next(-400, 401);
                vector2.Y -= 100 * num108;
                num78 = Main.mouseX + Main.screenPosition.X - vector2.X;
                num79 = Main.mouseY + Main.screenPosition.Y - vector2.Y;
                if (num79 < 0f)
                {
                    num79 *= -1f;
                }
                if (num79 < 20f)
                {
                    num79 = 20f;
                }
                num80 = (float)Math.Sqrt(num78 * num78 + num79 * num79);
                num80 = num72 / num80;
                Projectile.NewProjectile(source, vector2, Vector2.Zero, ModContent.ProjectileType<EssenceFlame2>(), (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f) * 0.25f), 0f, i, 0f, Main.rand.Next(3));
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
