using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
	public class AbyssShocker : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Abyss Shocker");
			Tooltip.SetDefault("Fires an erratic lightning bolt that arcs and bounces between enemies");
		}

		public override void SetDefaults() 
		{
			item.damage = 15;
            item.noMelee = true;
            item.magic = true;
            item.channel = true;
            item.width = 86;
			item.height = 32;
			item.useTime = 50;
			item.useAnimation = 20;
            item.UseSound = SoundID.Item13;
            item.useStyle = 5;
            item.mana = 10;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.Calamity().postMoonLordRarity = 21;
            item.shoot = ModContent.ProjectileType<LightningArc>();
            item.shootSpeed = 14f;
		}

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/AbyssShocker_mask");
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    item.position.X - Main.screenPosition.X + item.width * 0.5f,
                    item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-14, 0);
        }
    }
}