using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.DevourerofGods
{
    public class Excelsus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excelsus");
            Tooltip.SetDefault("Summons laser fountains on enemy hits");
        }

        public override void SetDefaults()
        {
            item.width = 78;
            item.damage = 235;
            item.melee = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 94;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("Excelsus");
            item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 origin = new Vector2(39f, 47f);
			spriteBatch.Draw(mod.GetTexture("Items/Weapons/DevourerofGods/ExcelsusGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
		}

		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
				switch (index)
				{
					case 0:
						type = mod.ProjectileType("Excelsus");
						break;
					case 1:
						type = mod.ProjectileType("ExcelsusBlue");
						break;
					case 2:
						type = mod.ProjectileType("ExcelsusPink");
						break;
				}
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("LaserFountain"), 0, 0, Main.myPlayer);
        }
    }
}
