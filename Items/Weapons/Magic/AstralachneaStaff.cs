using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class AstralachneaStaff : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
            Item.damage = 55;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.useAnimation = Item.useTime = 21;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item46;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AstralachneaFang>();
            Item.shootSpeed = 13f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 realPlayerPos = player.RotatedRelativePoint(player.MountedCenter, true);
            float mouseXDist = (float)Main.mouseX + Main.screenPosition.X - realPlayerPos.X;
            float mouseYDist = (float)Main.mouseY + Main.screenPosition.Y - realPlayerPos.Y;

            int spikeAmount = Main.rand.Next(3, 4+1);
            for (int j = 0; j < spikeAmount; j++)
            {
                Vector2 fangSpawn = new Vector2(mouseXDist, mouseYDist);
                fangSpawn.X += Main.rand.NextFloat(-20f, 20f) * j;
                fangSpawn.Y += Main.rand.NextFloat(-20f, 20f) * j;
                fangSpawn = fangSpawn.SafeNormalize(Vector2.UnitX) * Item.shootSpeed;

                Projectile.NewProjectile(source, realPlayerPos, fangSpawn, ModContent.ProjectileType<AstralachneaFang>(), damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
