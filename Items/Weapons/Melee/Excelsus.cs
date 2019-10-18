using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Excelsus : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Excelsus");
            Tooltip.SetDefault("Fires a spread of spinning blades\n" +
                "Summons laser fountains on hit");
        }

        public override void SetDefaults()
        {
            item.width = 78;
            item.damage = 340;
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
            item.shoot = ModContent.ProjectileType<ExcelsusMain>();
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(39f, 47f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/ExcelsusGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
                switch (index)
                {
                    case 0:
                        type = ModContent.ProjectileType<ExcelsusMain>();
                        break;
                    case 1:
                        type = ModContent.ProjectileType<ExcelsusBlue>();
                        break;
                    case 2:
                        type = ModContent.ProjectileType<ExcelsusPink>();
                        break;
                }
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<LaserFountain>(), 0, 0, Main.myPlayer);
        }
    }
}
