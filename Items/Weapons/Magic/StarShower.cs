using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("Starfall")]
    public class StarShower : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        internal const float ShootSpeed = 28f;
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 25;
            Item.mana = 15;
            Item.rare = ItemRarityID.Cyan;
            Item.useTime = 14;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3.25f;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.UseSound = SoundID.Item105;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralStarMagic>();
            Item.shootSpeed = ShootSpeed;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouse = player.ClampedMouseWorld();
            Vector2 destination = mouse;
            position = destination - Vector2.UnitY * (mouse.Y - Main.screenPosition.Y + 80f);
            Vector2 cachedPosition = position;
            float maxRandomOffset = 16f;
            int totalProjectiles = 5;
            for (int i = 0; i < totalProjectiles; i++)
            {
                position.X += MathHelper.Lerp(-160f, 160f, i / (float)(totalProjectiles - 1));
                position += Main.rand.NextVector2Circular(maxRandomOffset, maxRandomOffset);
                velocity = (mouse - position).SafeNormalize(Vector2.UnitY) * ShootSpeed * Main.rand.NextFloat(0.9f, 1.1f);
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                position = cachedPosition;
            }
            return false;
        }
    }
}
