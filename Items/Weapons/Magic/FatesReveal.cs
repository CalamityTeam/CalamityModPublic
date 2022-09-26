using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class FatesReveal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fate's Reveal");
            Tooltip.SetDefault("Spawns ghostly fireballs that follow the player");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 20;
            Item.width = 80;
            Item.height = 86;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FatesRevealFlame>();
            Item.shootSpeed = 1f;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Magic/FatesRevealGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
            if (player.gravDir == -1f)
            {
                num79 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
            }
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            if ((float.IsNaN(num78) && float.IsNaN(num79)) || (num78 == 0f && num79 == 0f))
            {
                num78 = (float)player.direction;
                num79 = 0f;
                num80 = Item.shootSpeed;
            }
            else
            {
                num80 = Item.shootSpeed / num80;
            }
            vector += new Vector2(num78, num79);

            int numProjectiles = 5;
            for (int i = 0; i < numProjectiles; i++)
            {
                vector.X += (float)Main.rand.Next(-100, 101);
                vector.Y += (float)(Main.rand.Next(-25, 26) * i);
                float spawnX = vector.X;
                float spawnY = vector.Y;
                Projectile.NewProjectile(source, spawnX, spawnY, 0f, 0f, type, damage, knockback, player.whoAmI, 0f, (float)Main.rand.Next(3));
            }
            return false;
        }
    }
}
