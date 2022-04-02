using CalamityMod.Projectiles.Melee;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Murasama : ModItem
    {
        public int frameCounter = 0;
        public int frame = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Murasama");
            Tooltip.SetDefault("There will be blood!\n" +
                "ID and power-level locked\n" +
                "Prove your strength or have the correct user ID to wield this sword");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 13));
        }

        public override void SetDefaults()
        {
            item.height = 128;
            item.width = 56;
            item.damage = 3001;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.useTime = 5;
            item.knockBack = 6.5f;
            item.autoReuse = false;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<MurasamaSlash>();
            item.shootSpeed = 24f;
            item.Calamity().customRarity = CalamityRarity.Violet;
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 14));
            item.Calamity().trueMelee = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 30;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            //0 = 6 frames, 8 = 3 frames]
            Texture2D texture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(texture, position, item.GetCurrentFrame(ref frame, ref frameCounter, frame == 0 ? 36 : frame == 8 ? 24 : 6, 13), Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture(Texture);
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, frame == 0 ? 36 : frame == 8 ? 24 : 6, 13), lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/MurasamaGlow");
            spriteBatch.Draw(texture, item.position - Main.screenPosition, item.GetCurrentFrame(ref frame, ref frameCounter, frame == 0 ? 36 : frame == 8 ? 24 : 6, 13, false), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[item.shoot] > 0)
                return false;
            return CalamityWorld.downedYharon || player.name == "Jetstream Sam" || player.name == "Samuel Rodrigues";
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
