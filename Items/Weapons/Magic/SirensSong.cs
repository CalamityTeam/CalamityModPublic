using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float xDist = Main.mouseX + Main.screenPosition.X - position.X;
            float yDist = Main.mouseY + Main.screenPosition.Y - position.Y;
            Vector2 mouseDist = new Vector2(xDist, yDist);
            float soundMult = mouseDist.Length() / (Main.screenHeight / 2f);
            if (soundMult > 1f)
                soundMult = 1f;
            float soundPitch = soundMult * 2f - 1f;
            soundPitch = MathHelper.Clamp(soundPitch, -1f, 1f);

            velocity.X += Main.rand.NextFloat(-0.75f, 0.75f);
            velocity.Y += Main.rand.NextFloat(-0.75f, 0.75f);
            velocity.X *= soundMult + 0.25f;
            velocity.Y *= soundMult + 0.25f;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, soundPitch, 0f);
            return false;
        }
    }
}
