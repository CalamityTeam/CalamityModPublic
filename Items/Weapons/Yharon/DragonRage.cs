using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Yharon
{
    public class DragonRage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon Rage");
        }

        public override void SetDefaults()
        {
            item.width = 68;
            item.damage = 683;
            item.melee = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 7.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 80;
            item.maxStack = 1;
            item.value = 10000000;
            item.shoot = mod.ProjectileType("DragonRage");
            item.shootSpeed = 14f;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "ItemName")
                {
                    line2.overrideColor = new Color(43, 96, 222);
                }
            }
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedX = speedX + 10f * 0.05f;
            float SpeedY = speedY + 10f * 0.05f;
            float SpeedX2 = speedX - 10f * 0.05f;
            float SpeedY2 = speedY - 10f * 0.05f;
            float SpeedX3 = speedX + 0f * 0.05f;
            float SpeedY3 = speedY + 0f * 0.05f;
            float SpeedX4 = speedX - 20f * 0.05f;
            float SpeedY4 = speedY - 20f * 0.05f;
            float SpeedX5 = speedX + 20f * 0.05f;
            float SpeedY5 = speedY + 20f * 0.05f;
            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, type, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, type, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX4, SpeedY4, type, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
            Projectile.NewProjectile(position.X, position.Y, SpeedX5, SpeedY5, type, (int)((double)damage * 0.75f), knockBack, player.whoAmI, 0.0f, 0.0f);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.Next(3) == 0)
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 360);
        }
    }
}
