using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class AmidiasPendant : ModItem
    {
        public const int ShardProjectiles = 2;
        public const float ShardAngleSpread = 120;
        public int ShardCountdown = 0;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Amidias' Pendant");
            Tooltip.SetDefault("Periodically rains down prism shards that can briefly stun enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 46;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (ShardCountdown <= 0)
            {
                ShardCountdown = 140;
            }
            if (ShardCountdown > 0)
            {
                ShardCountdown -= Main.rand.Next(1,4);
                if (ShardCountdown <= 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        var source = player.GetSource_Accessory(Item);
                        int speed2 = 25;
                        float spawnX = Main.rand.Next(1000) - 500 + player.Center.X;
                        float spawnY = -1000 + player.Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX, spawnY);
                        Vector2 baseVelocity = player.Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;
                        for (int i = 0; i < ShardProjectiles; i++)
                        {
                            Vector2 spawn = baseSpawn;
                            spawn.X = spawn.X + i * 30 - (ShardProjectiles * 15);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-ShardAngleSpread / 2 + (ShardAngleSpread * i / (float)ShardProjectiles)));
                            velocity.X = velocity.X + 3 * Main.rand.NextFloat() - 1.5f;
                            int type = 0;
                            int damage = 0;
                            switch (Main.rand.Next(3))
                            {
                                case 0:
                                    type = ModContent.ProjectileType<PendantProjectile1>();
                                    damage = 15;
                                    break;
                                case 1:
                                    type = ModContent.ProjectileType<PendantProjectile2>();
                                    damage = 15;
                                    break;
                                case 2:
                                    type = ModContent.ProjectileType<PendantProjectile3>();
                                    damage = 30;
                                    break;
                            }
                            int finalDamage = (int)player.GetBestClassDamage().ApplyTo(damage);
                            Projectile.NewProjectile(source, spawn.X, spawn.Y, velocity.X / 3, velocity.Y / 2, type, finalDamage, 5f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
        }
    }
}
