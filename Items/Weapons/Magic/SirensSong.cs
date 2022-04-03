using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class SirensSong : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anahita's Arpeggio");
            Tooltip.SetDefault("Casts slow-moving treble clefs that confuse enemies");
        }

        public override void SetDefaults()
        {
            Item.damage = 92;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.width = 56;
            Item.height = 50;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6.5f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SirensSongNote>();
            Item.shootSpeed = 13f;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-10, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float xDist = Main.mouseX + Main.screenPosition.X - position.X;
            float yDist = Main.mouseY + Main.screenPosition.Y - position.Y;
            Vector2 mouseDist = new Vector2(xDist, yDist);
            float soundMult = mouseDist.Length() / (Main.screenHeight / 2f);
            if (soundMult > 1f)
                soundMult = 1f;
            float soundPitch = soundMult * 2f - 1f;
            soundPitch = MathHelper.Clamp(soundPitch, -1f, 1f);

            speedX += Main.rand.NextFloat(-0.75f, 0.75f);
            speedY += Main.rand.NextFloat(-0.75f, 0.75f);
            speedX *= soundMult + 0.25f;
            speedY *= soundMult + 0.25f;

            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, soundPitch, 0f);
            return false;
        }
    }
}
