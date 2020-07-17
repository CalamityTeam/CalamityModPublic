using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class DeathhailStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathhail Staff");
            Tooltip.SetDefault("Rain death upon your foes!\n" +
                "Casts a storm of nebula lasers from the sky");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 180;
            item.magic = true;
            item.mana = 12;
            item.width = 80;
            item.height = 84;
            item.useTime = 6;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item12;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MagicNebulaShot>();
            item.shootSpeed = 18f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        /*public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }*/

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Vector2 origin = new Vector2(40f, 42f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Items/Weapons/Magic/DeathhailStaffGlow"), item.Center - Main.screenPosition, null, Color.White, rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			CalamityUtils.ProjectileToMouse(player, 2, item.shootSpeed, 0f, 50f, type, damage, knockBack, player.whoAmI, true);
            return false;
        }
    }
}
